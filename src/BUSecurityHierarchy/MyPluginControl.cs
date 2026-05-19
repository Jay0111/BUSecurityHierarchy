using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using ClosedXML.Excel;
using System.IO;

namespace BUSecurityHierarchy
{
    public partial class MyPluginControl : PluginControlBase
    {
        public MyPluginControl()
        {
            InitializeComponent();
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
        }


        #region Load BU Hierarchy

        private void btnLoadHierarchy_Click(object sender, EventArgs e)
        {
            // ExecuteMethod checks: "Are you connected?"
            // If NOT connected → shows "Please connect to an organization first"
            // If connected → runs LoadBUHierarchy()
            ExecuteMethod(LoadBUHierarchy);
        }

        private void LoadBUHierarchy()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading Business Unit hierarchy...",
                Work = (worker, args) =>
                {
                    try
                    {
                        var query = new QueryExpression("businessunit")
                        {
                            ColumnSet = new ColumnSet("businessunitid", "name", "parentbusinessunitid"),
                            Orders = { new OrderExpression("name", OrderType.Ascending) }
                        };
                        args.Result = Service.RetrieveMultiple(query);
                    }
                    catch (Exception ex)
                    {
                        // This will be available as args.Error in PostWorkCallBack
                        throw new Exception($"Failed to load Business Units. " +
                            $"Ensure you have 'Read' privilege on Business Unit entity.\n\n" +
                            $"Details: {ex.Message}", ex);
                    }
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        ShowErrorDialog(args.Error, "Security or Connection Error");
                        return;
                    }

                    var entities = ((EntityCollection)args.Result).Entities;

                    if (entities.Count == 0)
                    {
                        MessageBox.Show(
                            "No Business Units found. This could mean:\n\n" +
                            "• Your security role doesn't have Read access to Business Units\n" +
                            "• Your BU scope is restricted",
                            "No Data",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                    BuildBUTree(entities);
                }
            });
        }

        private void BuildBUTree(DataCollection<Entity> businessUnits)
        {
            treeViewBU.BeginUpdate();
            treeViewBU.Nodes.Clear();

            var rootBU = businessUnits.FirstOrDefault(bu =>
                !bu.Contains("parentbusinessunitid"));

            if (rootBU == null)
            {
                // User might only see their own BU (not root)
                // Fallback: treat the first BU as root
                rootBU = businessUnits.FirstOrDefault();
                if (rootBU == null)
                {
                    treeViewBU.EndUpdate();
                    return;
                }
            }

            var rootNode = CreateBUNode(rootBU);
            treeViewBU.Nodes.Add(rootNode);
            AddChildNodes(rootNode, rootBU.Id, businessUnits);
            rootNode.Expand();
            treeViewBU.EndUpdate();

            // Show count in label
            lblBU.Text = $"📂 Business Units ({businessUnits.Count})";
        }

        private void AddChildNodes(TreeNode parentNode, Guid parentBUId,
            DataCollection<Entity> allBUs)
        {
            var children = allBUs.Where(bu =>
                bu.Contains("parentbusinessunitid") &&
                ((EntityReference)bu["parentbusinessunitid"]).Id == parentBUId);

            foreach (var childBU in children)
            {
                var childNode = CreateBUNode(childBU);
                parentNode.Nodes.Add(childNode);
                AddChildNodes(childNode, childBU.Id, allBUs);
            }
        }

        private TreeNode CreateBUNode(Entity bu)
        {
            var name = bu.GetAttributeValue<string>("name") ?? "(No Name)";
            var node = new TreeNode("📁 " + name)
            {
                Tag = bu.Id,
                Name = bu.Id.ToString()
            };
            return node;
        }

        #endregion

        #region Expand / Collapse

        private void btnExpandAll_Click(object sender, EventArgs e)
        {
            treeViewBU.ExpandAll();
        }

        private void btnCollapseAll_Click(object sender, EventArgs e)
        {
            treeViewBU.CollapseAll();
        }

        #endregion

        #region BU Selected → Load Teams

        private void treeViewBU_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag == null) return;

            var selectedBUId = (Guid)e.Node.Tag;
            listViewTeams.Items.Clear();
            listViewUsers.Items.Clear();

            LoadTeamsForBU(selectedBUId);
        }

        private void LoadTeamsForBU(Guid businessUnitId)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading Teams...",
                Work = (worker, args) =>
                {
                    try
                    {
                        var teamQuery = new QueryExpression("team")
                        {
                            ColumnSet = new ColumnSet("name", "teamtype"),
                            Criteria = new FilterExpression
                            {
                                Conditions =
                                {
                                    new ConditionExpression("businessunitid",
                                        ConditionOperator.Equal, businessUnitId)
                                }
                            },
                            Orders = { new OrderExpression("name", OrderType.Ascending) }
                        };
                        var teams = Service.RetrieveMultiple(teamQuery);

                        var userQuery = new QueryExpression("systemuser")
                        {
                            ColumnSet = new ColumnSet("fullname", "internalemailaddress", "systemuserid"),
                            Criteria = new FilterExpression
                            {
                                Conditions =
                                {
                                    new ConditionExpression("businessunitid",
                                        ConditionOperator.Equal, businessUnitId),
                                     new ConditionExpression("isdisabled", ConditionOperator.Equal, false)
                                }
                            },
                            Orders = { new OrderExpression("fullname", OrderType.Ascending) }

                        };
                        var users = Service.RetrieveMultiple(userQuery);

                        args.Result = new { Teams = teams, Users = users };
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Failed to load Teams. " +
                            $"Ensure you have 'Read' privilege on Team entity.\n\n" +
                            $"Details: {ex.Message}", ex);
                    }
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        ShowErrorDialog(args.Error, "Error in loading Teams Or Users : " + args.Error.Message);
                        return;
                    }

                    dynamic result = args.Result;
                    EntityCollection teams = result.Teams;
                    EntityCollection users = result.Users;

                    listViewTeams.Items.Clear();
                    listViewUsers.Items.Clear();

                    // Populate Teams
                    foreach (var team in teams.Entities)
                    {
                        var name = team.GetAttributeValue<string>("name") ?? "";
                        var teamType = team.GetAttributeValue<OptionSetValue>("teamtype");
                        var typeLabel = teamType?.Value == 0 ? "Owner" :
                                        teamType?.Value == 1 ? "Access" :
                                        teamType?.Value == 2 ? "AAD Security" :
                                        teamType?.Value == 3 ? "AAD Office" : "Other";

                        var item = new ListViewItem(name) { Tag = team.Id };
                        item.SubItems.Add(typeLabel);
                        listViewTeams.Items.Add(item);
                    }
                    DisplayUsers(users.Entities.ToList());
                    lblUsers.Text = $"👤 Users ({users.Entities.Count})";
                    lblTeams.Text = $"👥 Teams ({teams.Entities.Count})";
                }
            });
        }

        #endregion

        #region Team Selected → Load Users

        private void listViewTeams_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewTeams.SelectedItems.Count == 0) return;

            var selectedTeamId = (Guid)listViewTeams.SelectedItems[0].Tag;
            listViewUsers.Items.Clear();

            LoadUsersForTeam(selectedTeamId);
        }

        private void LoadUsersForTeam(Guid teamId)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading Users...",
                Work = (worker, args) =>
                {
                    try
                    {
                        var query = new QueryExpression("systemuser")
                        {
                            ColumnSet = new ColumnSet("fullname", "internalemailaddress"),
                            LinkEntities =
                            {
                                new LinkEntity("systemuser", "teammembership",
                                    "systemuserid", "systemuserid", JoinOperator.Inner)
                                {
                                    LinkCriteria = new FilterExpression
                                    {
                                        Conditions =
                                        {
                                            new ConditionExpression("teamid",
                                                ConditionOperator.Equal, teamId)
                                        }
                                    }
                                }
                            },
                            Orders = { new OrderExpression("fullname", OrderType.Ascending) }
                        };
                        args.Result = Service.RetrieveMultiple(query);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Failed to load Users. " +
                            $"Ensure you have 'Read' privilege on User entity.\n\n" +
                            $"Details: {ex.Message}", ex);
                    }
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        ShowErrorDialog(args.Error, "User Load Error");
                        return;
                    }

                    var users = ((EntityCollection)args.Result).Entities;
                    listViewUsers.Items.Clear();

                    foreach (var user in users)
                    {
                        var name = user.GetAttributeValue<string>("fullname") ?? "";
                        var email = user.GetAttributeValue<string>("internalemailaddress") ?? "";

                        var item = new ListViewItem(name);
                        item.SubItems.Add(email);
                        listViewUsers.Items.Add(item);
                    }

                    lblUsers.Text = $"👤 Users ({users.Count})";
                }
            });
        }

        private void DisplayUsers(IEnumerable<Entity> users)
        {
            listViewUsers.Items.Clear();
            if (users == null) return;
            foreach (var user in users)
            {
                var item = new ListViewItem(user.GetAttributeValue<string>("fullname") ?? "N/A");
                item.SubItems.Add(user.GetAttributeValue<string>("internalemailaddress") ?? "");
                item.Tag = user.Id; // Store user ID
                listViewUsers.Items.Add(item);
            }
           
            // Update status or label if you have one
            // lblUsers.Text = $"Users ({listViewUsers.Items.Count})";
        }
        #endregion

        #region Export
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (listViewTeams.SelectedItems.Count > 0)
            {
                //Export selected team and Users
                ExportSelectedTeam();
            }
            else if (treeViewBU.SelectedNode != null)
            {
                //Export all teams and Users
                ExportAllUsersTeams();

            }
            else
            {
                MessageBox.Show("Please select a Business Unit or Team to export.", "No Selected",
                                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void ExportSelectedTeam()
        { 
            // Get selected team
            var selectedTeam = listViewTeams.SelectedItems[0];
            string teamName = selectedTeam.SubItems[0].Text;
            string teamType = selectedTeam.SubItems.Count > 1 ? selectedTeam.SubItems[1].Text : "";

            // File save dialog
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Excel Files|*.xlsx";
                saveDialog.Title = "Export Team and Users";
                saveDialog.FileName = $"{SanitizeFileName(teamName)}_Users_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                if (saveDialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Team Users");

                        // Header: Team Information
                        worksheet.Cell("A1").Value = "Team Name:";
                        worksheet.Cell("B1").Value = teamName;
                        worksheet.Cell("A2").Value = "Team Type:";
                        worksheet.Cell("B2").Value = teamType;
                        worksheet.Cell("A3").Value = "Export Date:";
                        worksheet.Cell("B3").Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        worksheet.Cell("A4").Value = "Total Users:";
                        worksheet.Cell("B4").Value = listViewUsers.Items.Count;

                        // Style team info
                        worksheet.Range("A1:A4").Style.Font.Bold = true;
                        worksheet.Range("A1:B4").Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Range("A1:B4").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        // User table headers (starting at row 6)
                        worksheet.Cell("A6").Value = "User Name";
                        worksheet.Cell("B6").Value = "Email";

                        var headerRange = worksheet.Range("A6:B6");
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
                        headerRange.Style.Font.FontColor = XLColor.White;
                        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

                        // Populate user data
                        int row = 7;
                        foreach (ListViewItem userItem in listViewUsers.Items)
                        {
                            worksheet.Cell(row, 1).Value = userItem.SubItems[0].Text; // User Name
                            worksheet.Cell(row, 2).Value = userItem.SubItems.Count > 1 ? userItem.SubItems[1].Text : ""; //Email
                                                row++;
                        }

                        // Format user data table
                        if (listViewUsers.Items.Count > 0)
                        {
                            var dataRange = worksheet.Range($"A7:B{row - 1}");
                            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                        }

                        // Auto-fit columns
                        worksheet.Columns().AdjustToContents();

                        // Save workbook
                        workbook.SaveAs(saveDialog.FileName);
                    }

                    MessageBox.Show($"Successfully exported {listViewUsers.Items.Count} users to:\n{saveDialog.FileName}",
                        "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting to Excel:\n{ex.Message}\n\n{ex.StackTrace}",
                        "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ExportAllUsersTeams()
        {
            var selectedBUNode = treeViewBU.SelectedNode;
            string buName = selectedBUNode.Text.Replace("📁 ", "").Replace("📂 ", "");
            Guid buId = (Guid)selectedBUNode.Tag;

            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Excel Files|*.xlsx";
                saveDialog.Title = "Export All Teams and Users";
                saveDialog.FileName = $"{SanitizeFileName(buName)}_AllTeams_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                if (saveDialog.ShowDialog() != DialogResult.OK)
                    return;

                WorkAsync(new WorkAsyncInfo
                {
                    Message = "Exporting all teams and users...",
                    Work = (worker, args) =>
                    {
                        try
                        {
                            // Get all teams for this BU (same as UI)
                            var teamQuery = new QueryExpression("team")
                            {
                                ColumnSet = new ColumnSet("teamid", "name", "teamtype"),
                                Criteria = new FilterExpression
                                {
                                    Conditions =
                              {
                                  new ConditionExpression("businessunitid", ConditionOperator.Equal, buId)
                              }
                                },
                                Orders = { new OrderExpression("name", OrderType.Ascending) }
                            };
                            var teams = Service.RetrieveMultiple(teamQuery).Entities;

                            // Get all users for this BU (same as UI - match what's displayed)
                            var userQuery = new QueryExpression("systemuser")
                            {
                                ColumnSet = new ColumnSet("systemuserid", "fullname", "internalemailaddress"),
                                Criteria = new FilterExpression
                                {
                                    Conditions =
                              {
                                  new ConditionExpression("businessunitid", ConditionOperator.Equal, buId),
                                  new ConditionExpression("isdisabled", ConditionOperator.Equal, false)
                              }
                                },
                                Orders = { new OrderExpression("fullname", OrderType.Ascending) }
                            };
                            var users = Service.RetrieveMultiple(userQuery).Entities;

                            args.Result = new
                            {
                                BUName = buName,
                                Teams = teams,
                                Users = users,
                                FilePath = saveDialog.FileName
                            };
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Failed to retrieve data: {ex.Message}", ex);
                        }
                    },
                    PostWorkCallBack = (args) =>
                    {
                        if (args.Error != null)
                        {
                            MessageBox.Show($"Error retrieving data:\n{args.Error.Message}",
                                "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        try
                        {
                            dynamic result = args.Result;
                            string buNameExport = result.BUName;
                            var teams = (DataCollection<Entity>)result.Teams;
                            var users = (DataCollection<Entity>)result.Users;
                            string filePath = result.FilePath;

                            using (var workbook = new XLWorkbook())
                            {
                                // Sheet 1: Summary
                                var summarySheet = workbook.Worksheets.Add("Summary");
                                summarySheet.Cell("A1").Value = "Business Unit:";
                                summarySheet.Cell("B1").Value = buNameExport;
                                summarySheet.Cell("A2").Value = "Export Date:";
                                summarySheet.Cell("B2").Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                summarySheet.Cell("A3").Value = "Total Teams:";
                                summarySheet.Cell("B3").Value = teams.Count;
                                summarySheet.Cell("A4").Value = "Total Users:";
                                summarySheet.Cell("B4").Value = users.Count;

                                summarySheet.Range("A1:A4").Style.Font.Bold = true;
                                summarySheet.Range("A1:B4").Style.Fill.BackgroundColor = XLColor.LightBlue;
                                summarySheet.Columns().AdjustToContents();

                                // Sheet 2: All Teams
                                var teamsSheet = workbook.Worksheets.Add("Teams");
                                teamsSheet.Cell("A1").Value = "Team Name";
                                teamsSheet.Cell("B1").Value = "Team Type";

                                var teamHeaderRange = teamsSheet.Range("A1:B1");
                                teamHeaderRange.Style.Font.Bold = true;
                                teamHeaderRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
                                teamHeaderRange.Style.Font.FontColor = XLColor.White;

                                int teamRow = 2;
                                foreach (var team in teams)
                                {
                                    var teamType = team.GetAttributeValue<OptionSetValue>("teamtype");
                                    var typeLabel = teamType?.Value == 0 ? "Owner" :
                                                  teamType?.Value == 1 ? "Access" :
                                                  teamType?.Value == 2 ? "AAD Security" :
                                                  teamType?.Value == 3 ? "AAD Office" : "Other";

                                    teamsSheet.Cell(teamRow, 1).Value = team.GetAttributeValue<string>("name");
                                    teamsSheet.Cell(teamRow, 2).Value = typeLabel;
                                    teamRow++;
                                }
                                teamsSheet.Columns().AdjustToContents();

                                // Sheet 3: All Users
                                var usersSheet = workbook.Worksheets.Add("Users");
                                usersSheet.Cell("A1").Value = "User Name";
                                usersSheet.Cell("B1").Value = "Email";

                                var userHeaderRange = usersSheet.Range("A1:B1");
                                userHeaderRange.Style.Font.Bold = true;
                                userHeaderRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
                                userHeaderRange.Style.Font.FontColor = XLColor.White;

                                int userRow = 2;
                                foreach (var user in users)
                                {
                                    usersSheet.Cell(userRow, 1).Value = user.GetAttributeValue<string>("fullname");
                                    usersSheet.Cell(userRow, 2).Value =
        user.GetAttributeValue<string>("internalemailaddress");
                                    userRow++;
                                }
                                usersSheet.Columns().AdjustToContents();

                                // Save workbook
                                workbook.SaveAs(filePath);
                            }

                            MessageBox.Show($"Successfully exported:\n" +
                                $"• {teams.Count} teams\n" +
                                $"• {users.Count} users\n\n" +
                                $"File: {filePath}",
                                "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error creating Excel file:\n{ex.Message}\n\n{ex.StackTrace}",
                                "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                });
            }
        }

       

        private string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
        }

        #endregion

        #region Helper: Error Dialog

        /// <summary>
        /// Shows a detailed error dialog with friendly message
        /// </summary>
        private void ShowErrorDialog(Exception ex, string title)
        {
            var message = ex.Message;

            // Detect common Dataverse security errors
            if (message.Contains("SecLib::AccessCheckEx"))
            {
                message = "🔒 ACCESS DENIED\n\n" +
                    "Your Dynamics 365 security role does not have permission " +
                    "to read this data.\n\n" +
                    "Required privileges:\n" +
                    "• Business Unit → Read (Organization level)\n" +
                    "• Team → Read (Organization level)\n" +
                    "• User → Read (Organization level)\n\n" +
                    "Contact your System Administrator to update your security role.";
            }
            else if (message.Contains("401") || message.Contains("Unauthorized"))
            {
                message = "🔒 AUTHENTICATION EXPIRED\n\n" +
                    "Your session has expired. Please reconnect to the organization.";
            }
            else if (message.Contains("403") || message.Contains("Forbidden"))
            {
                message = "🔒 FORBIDDEN\n\n" +
                    "You don't have access to this Dynamics 365 environment.";
            }

            MessageBox.Show(message, title,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion
    }
}
