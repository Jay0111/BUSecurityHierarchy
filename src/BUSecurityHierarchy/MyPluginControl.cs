using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

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
                        var query = new QueryExpression("team")
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
                        args.Result = Service.RetrieveMultiple(query);
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
                        ShowErrorDialog(args.Error, "Team Load Error");
                        return;
                    }

                    var teams = ((EntityCollection)args.Result).Entities;
                    listViewTeams.Items.Clear();

                    foreach (var team in teams)
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

                    lblTeams.Text = $"👥 Teams ({teams.Count})";
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

        #endregion

        #region Export

        private void btnExport_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Export feature coming soon!", "Export",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
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
