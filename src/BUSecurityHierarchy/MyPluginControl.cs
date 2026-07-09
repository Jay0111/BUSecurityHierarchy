using ClosedXML.Excel;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace BUSecurityHierarchy
{
    /// <summary>
    /// Helper class to store role information for search and display
    /// </summary>
    public class RoleItem
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string BusinessUnit { get; set; }
        public bool IsChecked { get; set; }

        public RoleItem(Guid roleId, string roleName, string businessUnit = "", bool isChecked = false)
        {
            RoleId = roleId;
            RoleName = roleName;
            BusinessUnit = businessUnit;
            IsChecked = isChecked;
        }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(BusinessUnit)
                ? RoleName
                : $"{RoleName} ({BusinessUnit})";
        }
    }
    public partial class MyPluginControl : PluginControlBase
    {
        public MyPluginControl()
        {
            InitializeComponent();
            // Subscribe to connection change event
            ConnectionUpdated += MyPluginControl_ConnectionUpdated;
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
            chkListRoles.Items.Clear();

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
                        // Retrieve teams
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

                        // Retrieve Users
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

                        // Retrieve Security Roles
                        var rolesQuery = new QueryExpression("role")
                        {
                            ColumnSet = new ColumnSet("name", "businessunitid", "roleid"),
                            Criteria = new FilterExpression
                            {
                                Conditions =
                                {
                                    new ConditionExpression ("businessunitid",
                                        ConditionOperator.Equal, businessUnitId)
                                }
                            },
                            Orders = {new OrderExpression("name", OrderType.Ascending) }
                        };
                        var roles = Service.RetrieveMultiple(rolesQuery);

                        args.Result = new { Teams = teams, Users = users, Roles = roles};
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Failed to load. " +
                            $"Ensure you have 'Read' privilege.\n\n" +
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
                    EntityCollection roles = result.Roles;

                    listViewTeams.Items.Clear();
                    listViewUsers.Items.Clear();
                    chkListRoles.Items.Clear();

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
                    DisplayRoles(roles.Entities.ToList());
                    lblUsers.Text = $"👤 Users ({users.Entities.Count})";
                    lblTeams.Text = $"👥 Teams ({teams.Entities.Count})";
                    lblRoles.Text = $"🛡️ Security Roles ({roles.Entities.Count})";

                    _selectedTeamId = null;
                    HideRoleSaveButton();
                }
            });
        }

        #endregion

        #region Team Selected → Load Users

        //private Guid? _selectedTeamId = null; // Track currently selected team
        //private bool _hasUnsavedChanges = false;
        private HashSet<Guid> _originalAssignedRoles = new HashSet<Guid>();
        //private int _currentLoadGeneration = 0;
        private void listViewTeams_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewTeams.SelectedItems.Count == 0)
            {
                _selectedTeamId = null;
                listViewUsers.Items.Clear();
                HideRoleSaveButton(); // Reset UI
                return;
            }

            var selectedTeamId = (Guid)listViewTeams.SelectedItems[0].Tag;
            _selectedTeamId = selectedTeamId;
            listViewUsers.Items.Clear();

            // ✅ CLEAR SEARCH AND REMOVE FOCUS BEFORE LOADING
            txtSearchRoles.TextChanged -= txtSearchRoles_TextChanged;
            txtSearchRoles.Text = "";
            txtSearchRoles.ForeColor = System.Drawing.Color.Gray;
            txtSearchRoles.TextChanged += txtSearchRoles_TextChanged;

            // ✅ MOVE FOCUS AWAY FROM SEARCH BOX
            listViewTeams.Focus();

            LoadUsersForTeam(selectedTeamId);
            CheckRolesForTeam(selectedTeamId);
        }

       

        private void chkListRoles_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // ✅ BLOCK CHANGES IF NO TEAM SELECTED
            if (_selectedTeamId == null)
            {
                e.NewValue = e.CurrentValue; // Revert the change
                MessageBox.Show("⚠️ Please select a Team first before assigning roles.",
                    "No Team Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ✅ SHOW SAVE BUTTON (use BeginInvoke because ItemCheck fires BEFORE the check state changes)
            this.BeginInvoke(new Action(() =>
            {
                // Get current checked roles
                var currentAssignedRoles = GetCurrentCheckedRoles();
                // Compare with original state
                bool hasChanges = !_originalAssignedRoles.SetEquals(currentAssignedRoles);

                if (hasChanges)
                {
                    ShowRoleSaveButton();
                }
                else
                {
                    HideRoleSaveButton();
                }
            }));
        }

        private void ShowRoleSaveButton()
        {
            _hasUnsavedChanges = true;
            btnSaveRoles.Visible = true;
            lblRoleChangesWarning.Visible = true;
        }

        // ✅ NEW: Hide save button and warning
        private void HideRoleSaveButton()
        {
            _hasUnsavedChanges = false;
            btnSaveRoles.Visible = false;
            lblRoleChangesWarning.Visible = false;
        }

        // ✅ NEW: Save role assignments
        private void btnSaveRoles_Click(object sender, EventArgs e)
        {
            if (_selectedTeamId == null)
            {
                MessageBox.Show("No team selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get current checked roles
            var currentAssignedRoles = GetCurrentCheckedRoles();

            // Calculate changes
            var rolesToAssign = currentAssignedRoles.Except(_originalAssignedRoles).ToList();
            var rolesToDeassign = _originalAssignedRoles.Except(currentAssignedRoles).ToList();

            if (rolesToAssign.Count == 0 && rolesToDeassign.Count == 0)
            {
                MessageBox.Show("No changes detected.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                HideRoleSaveButton();
                return;
            }

            // Confirm with user
            var confirmMessage = $"Apply the following changes?\n\n" +
                                 $"✅ Assign: {rolesToAssign.Count} role(s)\n" +
                                 $"❌ Remove: {rolesToDeassign.Count} role(s)";

            if (MessageBox.Show(confirmMessage, "Confirm Changes",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            SaveRoleChanges(_selectedTeamId.Value, rolesToAssign, rolesToDeassign);
        }

        /// <summary>
        /// Gets currently checked role IDs from the UI
        /// </summary>
        private HashSet<Guid> GetCurrentCheckedRoles()
        {
            var currentAssignedRoles = new HashSet<Guid>();
            foreach (int index in chkListRoles.CheckedIndices)
            {
                var roleItem = (RoleItem)chkListRoles.Items[index];
                currentAssignedRoles.Add(roleItem.RoleId);
            }
            return currentAssignedRoles;
        }

        private void SaveRoleChanges(Guid teamId, List<Guid> rolesToAssign, List<Guid> rolesToDeassign)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Saving role changes...",
                Work = (worker, args) =>
                {
                    try
                    {
                        int successCount = 0;
                        int errorCount = 0;
                        var errors = new List<string>();

                        // ✅ ASSIGN NEW ROLES
                        foreach (var roleId in rolesToAssign)
                        {
                            try
                            {
                                var teamReference = new EntityReference("team", teamId);
                                var roleReference = new EntityReference("role", roleId);

                                Service.Associate(
                                    "team",
                                    teamId,
                                    new Microsoft.Xrm.Sdk.Relationship("teamroles_association"),
                                    new EntityReferenceCollection { roleReference }
                                );
                                successCount++;
                            }
                            catch (Exception ex)
                            {
                                errorCount++;
                                errors.Add($"Assign {roleId}: {ex.Message}");
                            }
                        }

                        // ✅ DEASSIGN REMOVED ROLES
                        foreach (var roleId in rolesToDeassign)
                        {
                            try
                            {
                                var roleReference = new EntityReference("role", roleId);

                                Service.Disassociate(
                                    "team",
                                    teamId,
                                    new Microsoft.Xrm.Sdk.Relationship("teamroles_association"),
                                    new EntityReferenceCollection { roleReference }
                                );
                                successCount++;
                            }
                            catch (Exception ex)
                            {
                                errorCount++;
                                errors.Add($"Remove {roleId}: {ex.Message}");
                            }
                        }

                        args.Result = new
                        {
                            SuccessCount = successCount,
                            ErrorCount = errorCount,
                            Errors = errors
                        };
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Failed to save role changes: {ex.Message}", ex);
                    }
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        ShowErrorDialog(args.Error, "Save Error");
                        return;
                    }

                    dynamic result = args.Result;
                    int successCount = result.SuccessCount;
                    int errorCount = result.ErrorCount;
                    List<string> errors = result.Errors;

                    if (errorCount > 0)
                    {
                        var errorMessage = $"⚠️ Completed with errors:\n\n" +
                                           $"✅ Success: {successCount}\n" +
                                           $"❌ Errors: {errorCount}\n\n" +
                                           $"Details:\n{string.Join("\n", errors)}";
                        MessageBox.Show(errorMessage, "Partial Success", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show($"✅ Successfully saved {successCount} role change(s)!",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    // ✅ REFRESH ROLES TO SHOW UPDATED STATE
                    HideRoleSaveButton();
                    //CheckRolesForTeam(_selectedTeamId.Value);
                    if (_selectedTeamId.HasValue && _selectedTeamId.Value == teamId)
                    {
                        CheckRolesForTeam(teamId); 
                    }
                }
            });
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

        private void CheckRolesForTeam(Guid teamId)
        {
            if (teamId == Guid.Empty) return;

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading team roles..",
                Work = (worker, args) =>
                {
                    try
                    {
                        var teamRolesQuery = new QueryExpression("teamroles")
                        {
                            ColumnSet = new ColumnSet("roleid"),
                            Criteria = new FilterExpression
                            {
                                Conditions = { new ConditionExpression("teamid", ConditionOperator.Equal, teamId) }
                            },
                        };
                        var assignedRoles = Service.RetrieveMultiple(teamRolesQuery);
                        var assignedRoleIds = new HashSet<Guid>();

                        foreach (var teamRole in assignedRoles.Entities)
                        {
                            assignedRoleIds.Add(teamRole.GetAttributeValue<Guid>("roleid"));
                        }

                        args.Result = assignedRoleIds;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Failed to load Security Roles: {ex.Message}", ex);
                    }
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        ShowErrorDialog(args.Error, "Team roles Load Error");
                        return;
                    }

                    var assignedRoleIds = (HashSet<Guid>)args.Result;
                    _originalAssignedRoles = new HashSet<Guid>(assignedRoleIds);

                    if (_allRoles != null && _allRoles.Count > 0)
                    {
                        foreach (var role in _allRoles)
                        {
                            role.IsChecked = assignedRoleIds.Contains(role.RoleId);
                        }
                    }

                    // Sort: assigned roles first
                    var sortedRoles = _allRoles
                        .OrderByDescending(r => r.IsChecked)
                        .ThenBy(r => r.RoleName)
                        .ToList();

                    chkListRoles.ItemCheck -= chkListRoles_ItemCheck;
                    chkListRoles.Items.Clear();

                    foreach (var role in sortedRoles)
                    {
                        chkListRoles.Items.Add(role, role.IsChecked);
                    }

                    chkListRoles.ItemCheck += chkListRoles_ItemCheck;

                    lblRoles.Text = $"🛡️ Security Roles ({assignedRoleIds.Count} assigned)";

                    // ✅ CLEAR SEARCH PROPERLY - DISABLE EVENT FIRST
                    txtSearchRoles.TextChanged -= txtSearchRoles_TextChanged;
                    txtSearchRoles.Text = "";
                    txtSearchRoles.ForeColor = System.Drawing.Color.Gray;
                    txtSearchRoles.TextChanged += txtSearchRoles_TextChanged;

                    // ✅ REMOVE FOCUS FROM SEARCH BOX
                    chkListRoles.Focus();

                    HideRoleSaveButton();
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

        private void DisplayRoles(IEnumerable<Entity> roles)
        {
            chkListRoles.Items.Clear();
            _allRoles = new List<RoleItem>();

            if (roles == null) return;
            foreach (var role in roles)
            {
                var roleItem = new RoleItem(
                    roleId: role.Id,
                    roleName: role.GetAttributeValue<string>("name") ?? "N/A",
                    businessUnit: role.GetAttributeValue<EntityReference>("businessunitid")?.Name ?? "",
                     isChecked: false
                );
                _allRoles.Add(roleItem);
                chkListRoles.Items.Add(roleItem, false);
            }
            // Clear search box when loading new roles
            txtSearchRoles.Text = "";
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

            if (_hasUnsavedChanges)
            {
                MessageBox.Show("Please save or discard role changes before exporting.",
                    "Unsaved Role Changes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get checked roles from UI
            var assignedRoles = new List<RoleItem>();
            foreach (var item in chkListRoles.CheckedItems)
            {
                assignedRoles.Add((RoleItem)item);
            }

            // File save dialog
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Excel Files|*.xlsx";
                saveDialog.Title = "Export Team and Users and Assgined Roles";
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
                        worksheet.Cell("A5").Value = "Total Assigned Security Roles";
                        worksheet.Cell("B5").Value = chkListRoles.CheckedItems.Count;

                        // Style team info
                        worksheet.Range("A1:A5").Style.Font.Bold = true;
                        worksheet.Range("A1:B5").Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Range("A1:B5").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        // User table headers (starting at row 6)
                        worksheet.Cell("A7").Value = "User Name";
                        worksheet.Cell("B7").Value = "Email";

                        var headerRange = worksheet.Range("A7:B7");
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
                        headerRange.Style.Font.FontColor = XLColor.White;
                        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

                        // Populate user data
                        int row = 8;
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

                        worksheet.Cell($"A{row + 2}").Value = "Role Name";
                        worksheet.Cell($"B{row + 2}").Value = "Business Unit";


                        var roleHeaderRange = worksheet.Range($"A{row + 2}:B{row + 2}");
                        roleHeaderRange.Style.Font.Bold = true;
                        roleHeaderRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
                        roleHeaderRange.Style.Font.FontColor = XLColor.White;
                        roleHeaderRange.Style.Border.OutsideBorder  = XLBorderStyleValues.Medium;

                        // Populate user data
                        int row1 = row + 3;
                        foreach (RoleItem roleItem in chkListRoles.CheckedItems)
                        {
                            worksheet.Cell(row1, 1).Value = roleItem.RoleName; // Role Name
                            worksheet.Cell(row1, 2).Value = roleItem.BusinessUnit; // BU Name
                            row1++;
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
                    Message = "Exporting all teams and users and roles...",
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

                            var roleQuery = new QueryExpression("role")
                            {
                                ColumnSet = new ColumnSet("name", "roleid", "businessunitid"),
                                Criteria = new FilterExpression
                                {
                                    Conditions = { new ConditionExpression("businessunitid", ConditionOperator.Equal, buId) }
                                },
                                Orders = { new OrderExpression("name", OrderType.Ascending)  }
                            };

                            var roles = Service.RetrieveMultiple(roleQuery).Entities;
                            
                          

                            args.Result = new
                            {
                                BUName = buName,
                                Teams = teams,
                                Users = users,
                                Roles = roles,
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
                            var roles = (DataCollection<Entity>)result.Roles;
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
                                summarySheet.Cell("A5").Value = "Total Roles:";
                                summarySheet.Cell("B5").Value = roles.Count;

                                summarySheet.Range("A1:A5").Style.Font.Bold = true;
                                summarySheet.Range("A1:B5").Style.Fill.BackgroundColor = XLColor.LightBlue;
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
                                    usersSheet.Cell(userRow, 2).Value = user.GetAttributeValue<string>("internalemailaddress");
                                    userRow++;
                                }
                                usersSheet.Columns().AdjustToContents();

                                // Sheet 4: All Security Roles
                                var roleSheet = workbook.Worksheets.Add("Security Roles");
                                roleSheet.Cell("A1").Value = "Role Name";
                                roleSheet.Cell("B1").Value = "Business Unit";

                                var roleHeaderRange = roleSheet.Range("A1:B1");
                                roleHeaderRange.Style.Font.Bold = true;
                                roleHeaderRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
                                roleHeaderRange.Style.Font.FontColor = XLColor.White;

                                int roleRow = 2;
                                foreach(var role in roles)
                                {
                                    roleSheet.Cell(roleRow, 1).Value = role.GetAttributeValue<string>("name");
                                    roleSheet.Cell(roleRow, 2).Value = role.GetAttributeValue<EntityReference>("businessunitid")?.Name ?? "-";
                                    roleRow++;
                                }
                                roleSheet.Columns().AdjustToContents();

                                // Save workbook
                                workbook.SaveAs(filePath);
                            }

                            MessageBox.Show($"Successfully exported:\n" +
                                $"• {teams.Count} teams\n" +
                                $"• {users.Count} users\n" +
                                $"• {roles.Count} roles\n\n" +
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

        #region Connection Change Handling

        /// <summary>
        /// Called when user changes connection (Dev → UAT, etc.)
        /// </summary>
        private void MyPluginControl_ConnectionUpdated(object sender, ConnectionUpdatedEventArgs e)
        {
            //_currentLoadGeneration++;
            // Clear all UI when connection changes
            ClearAllUI();

            // Update status (similar to the example)
            // Note: We don't access e.ConnectionDetail to avoid CS0012 error
            SetStatusMessage("Connection changed. Please reload the Business Unit hierarchy.", System.Drawing.Color.DarkOrange);
        }

        private void ClearAllUI()
        {
            // Clear TreeView
            treeViewBU.Nodes.Clear();
            lblBU.Text = "📂 Business Units";

            // Clear Teams
            listViewTeams.Items.Clear();
            lblTeams.Text = "👥 Teams";

            // Clear Users
            listViewUsers.Items.Clear();
            lblUsers.Text = "👤 Users";

            // Clear Roles (disable event first to avoid triggering save button)
            chkListRoles.ItemCheck -= chkListRoles_ItemCheck;
            chkListRoles.Items.Clear();
            chkListRoles.ItemCheck += chkListRoles_ItemCheck;
            lblRoles.Text = "🛡️ Security Roles";

            // Reset state variables (like the example does)
            _selectedTeamId = null;
            _hasUnsavedChanges = false;
            _originalAssignedRoles.Clear();

            // Hide save button and warning
            HideRoleSaveButton();
        }

        // Helper method (similar to the example)
        private void SetStatusMessage(string message, System.Drawing.Color color)
        {
            // You can add a status label if you want
            // lblStatus.Text = message;
            // lblStatus.ForeColor = color;
        }

        #endregion

        #region Helper: Search Functionality

        /// <summary>
        /// Filters the roles CheckedListBox based on search text
        /// </summary>
        private void txtSearchRoles_TextChanged(object sender, EventArgs e)
        {
            FilterRoles(txtSearchRoles.Text);
        }

        /// <summary>
        /// Filters the roles list based on search query
        /// </summary>
        private void FilterRoles(string searchQuery)
        {
            if (_allRoles == null || _allRoles.Count == 0)
                return;

            // ✅ IGNORE PLACEHOLDER TEXT
            if (searchQuery == "Type to search roles...")
                searchQuery = "";

            // ✅ IGNORE IF CLEARING DURING PROGRAMMATIC UPDATE
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                // Only refresh if currently filtered
                if (chkListRoles.Items.Count != _allRoles.Count)
                {
                    RefreshRolesList();
                }
                return;
            }

            // Save current checked states before clearing
            SaveCurrentCheckedStates();

            // Clear the checkedlistbox
            chkListRoles.ItemCheck -= chkListRoles_ItemCheck;
            chkListRoles.Items.Clear();

            // Filter roles based on search query
            var filteredRoles = _allRoles
                .Where(r => r.RoleName.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            // Add filtered roles back to the checkedlistbox
            foreach (var role in filteredRoles)
            {
                chkListRoles.Items.Add(role, role.IsChecked);
            }

            chkListRoles.ItemCheck += chkListRoles_ItemCheck;
        }

        /// <summary>
        /// Refreshes the roles list without filtering (shows all roles)
        /// </summary>
        private void RefreshRolesList()
        {
            if (_allRoles == null || _allRoles.Count == 0)
                return;

            SaveCurrentCheckedStates();

            chkListRoles.ItemCheck -= chkListRoles_ItemCheck;
            chkListRoles.Items.Clear();

            foreach (var role in _allRoles)
            {
                chkListRoles.Items.Add(role, role.IsChecked);
            }

            chkListRoles.ItemCheck += chkListRoles_ItemCheck;
        }

        /// <summary>
        /// Saves the current checked states from the CheckedListBox to _allRoles
        /// </summary>
        private void SaveCurrentCheckedStates()
        {
            if (_allRoles == null)
                return;

            // Update the IsChecked property in _allRoles based on current CheckedListBox state
            foreach (var item in chkListRoles.Items)
            {
                if (item is RoleItem roleItem)
                {
                    var matchingRole = _allRoles.FirstOrDefault(r => r.RoleId == roleItem.RoleId);
                    if (matchingRole != null)
                    {
                        matchingRole.IsChecked = chkListRoles.CheckedItems.Contains(roleItem);
                    }
                }
            }
        }

        /// <summary>
        /// Clear placeholder text when user focuses on search box
        /// </summary>
        private void txtSearchRoles_Enter(object sender, EventArgs e)
        {
            if (txtSearchRoles.Text == "Type to search roles...")
            {
                // ✅ DISABLE EVENT BEFORE CLEARING
                txtSearchRoles.TextChanged -= txtSearchRoles_TextChanged;
                txtSearchRoles.Text = "";
                txtSearchRoles.ForeColor = System.Drawing.Color.Black;
                txtSearchRoles.TextChanged += txtSearchRoles_TextChanged;
            }
        }

        /// <summary>
        /// Restore placeholder text when search box loses focus and is empty
        /// </summary>
        private void txtSearchRoles_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearchRoles.Text))
            {
                // ✅ DISABLE EVENT BEFORE SETTING PLACEHOLDER
                txtSearchRoles.TextChanged -= txtSearchRoles_TextChanged;
                txtSearchRoles.Text = "Type to search roles...";
                txtSearchRoles.ForeColor = System.Drawing.Color.Gray;
                txtSearchRoles.TextChanged += txtSearchRoles_TextChanged;
            }
        }

        #endregion

    }
}
