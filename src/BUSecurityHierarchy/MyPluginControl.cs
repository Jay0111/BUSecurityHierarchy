using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using XrmToolBox.Extensibility;

namespace BUSecurityHierarchy
{
    public partial class MyPluginControl : PluginControlBase
    {
        private EntityCollection allBUs;
        private EntityCollection allTeams;
        private EntityCollection allUsers;

        public MyPluginControl()
        {
            InitializeComponent();
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            LogInfo("Plugin loaded.");
        }

        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail,
            string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);
            LogInfo("Connected to: " + detail.WebApplicationUrl);
        }

        // ========== LOAD DATA ==========
        private void btnLoadHierarchy_Click(object sender, EventArgs e)
        {
            ExecuteMethod(LoadBUHierarchy);
        }

        private void LoadBUHierarchy()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading Business Unit Hierarchy...",
                Work = (worker, args) =>
                {
                    // Fetch ONLY Active Business Units (isdisabled = false)
                    QueryExpression buQuery = new QueryExpression("businessunit");
                    buQuery.ColumnSet = new ColumnSet("name", "parentbusinessunitid", "businessunitid");
                    buQuery.Criteria.AddCondition("isdisabled", ConditionOperator.Equal, false);
                    buQuery.AddOrder("name", OrderType.Ascending);
                    EntityCollection buResults = Service.RetrieveMultiple(buQuery);

                    // Fetch Teams belonging to Active BUs only
                    QueryExpression teamQuery = new QueryExpression("team");
                    teamQuery.ColumnSet = new ColumnSet("name", "businessunitid", "teamid", "teamtype");
                    teamQuery.AddOrder("name", OrderType.Ascending);

                    // Link to active BUs only
                    LinkEntity teamBuLink = teamQuery.AddLink("businessunit", "businessunitid", "businessunitid");
                    teamBuLink.LinkCriteria.AddCondition("isdisabled", ConditionOperator.Equal, false);

                    EntityCollection teamResults = Service.RetrieveMultiple(teamQuery);

                    // Fetch ONLY Active Users (isdisabled = false)
                    QueryExpression userQuery = new QueryExpression("systemuser");
                    userQuery.ColumnSet = new ColumnSet("fullname", "businessunitid", "systemuserid", "internalemailaddress");
                    userQuery.Criteria.AddCondition("isdisabled", ConditionOperator.Equal, false);
                    userQuery.Criteria.AddCondition("accessmode", ConditionOperator.NotEqual, 3); // Exclude non-interactive users
                    userQuery.AddOrder("fullname", OrderType.Ascending);
                    EntityCollection userResults = Service.RetrieveMultiple(userQuery);

                    args.Result = new object[] { buResults, teamResults, userResults };
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    object[] results = (object[])args.Result;
                    allBUs = (EntityCollection)results[0];
                    allTeams = (EntityCollection)results[1];
                    allUsers = (EntityCollection)results[2];

                    PopulateBusinessUnits();

                    lblBU.Text = "Business Units (" + allBUs.Entities.Count + ")";
                    lblTeams.Text = "Teams";
                    lblUsers.Text = "Users";

                    LogInfo("Hierarchy loaded successfully.");
                }
            });
        }

        // ========== POPULATE BUSINESS UNITS ==========
        private void PopulateBusinessUnits()
        {
            listViewBU.Items.Clear();
            listViewTeams.Items.Clear();
            listViewUsers.Items.Clear();

            foreach (Entity bu in allBUs.Entities)
            {
                string buName = bu.GetAttributeValue<string>("name");
                Guid buId = bu.Id;

                ListViewItem item = new ListViewItem(buName);
                item.Tag = buId;
                listViewBU.Items.Add(item);
            }
        }

        // ========== BU SELECTED → SHOW TEAMS & USERS ==========
        private void listViewBU_SelectedIndexChanged(object sender, EventArgs e)
        {
            listViewTeams.Items.Clear();
            listViewUsers.Items.Clear();

            if (listViewBU.SelectedItems.Count == 0)
                return;

            Guid selectedBUId = (Guid)listViewBU.SelectedItems[0].Tag;
            string selectedBUName = listViewBU.SelectedItems[0].Text;

            // Filter Teams for selected BU
            int teamCount = 0;
            foreach (Entity team in allTeams.Entities)
            {
                EntityReference teamBuRef = team.GetAttributeValue<EntityReference>("businessunitid");
                if (teamBuRef != null && teamBuRef.Id == selectedBUId)
                {
                    string teamName = team.GetAttributeValue<string>("name");
                    OptionSetValue teamType = team.GetAttributeValue<OptionSetValue>("teamtype");
                    string teamTypeName = GetTeamTypeName(teamType);

                    ListViewItem item = new ListViewItem(teamName);
                    item.SubItems.Add(teamTypeName);
                    item.Tag = team.Id;
                    listViewTeams.Items.Add(item);
                    teamCount++;
                }
            }

            // Filter Users for selected BU
            int userCount = 0;
            foreach (Entity user in allUsers.Entities)
            {
                EntityReference userBuRef = user.GetAttributeValue<EntityReference>("businessunitid");
                if (userBuRef != null && userBuRef.Id == selectedBUId)
                {
                    string userName = user.GetAttributeValue<string>("fullname");
                    string email = user.GetAttributeValue<string>("internalemailaddress");

                    ListViewItem item = new ListViewItem(userName);
                    item.SubItems.Add(email ?? "");
                    item.Tag = user.Id;
                    listViewUsers.Items.Add(item);
                    userCount++;
                }
            }

            lblTeams.Text = "Teams (" + teamCount + ") - " + selectedBUName;
            lblUsers.Text = "Users (" + userCount + ") - " + selectedBUName;
        }

        // ========== TEAM SELECTED → SHOW TEAM MEMBERS ==========
        private void listViewTeams_SelectedIndexChanged(object sender, EventArgs e)
        {
            listViewUsers.Items.Clear();

            if (listViewTeams.SelectedItems.Count == 0)
                return;

            Guid selectedTeamId = (Guid)listViewTeams.SelectedItems[0].Tag;
            string selectedTeamName = listViewTeams.SelectedItems[0].Text;

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading Team Members...",
                Work = (worker, args) =>
                {
                    // Fetch team members using N:N relationship
                    QueryExpression memberQuery = new QueryExpression("systemuser");
                    memberQuery.ColumnSet = new ColumnSet("fullname", "internalemailaddress", "systemuserid");
                    memberQuery.Criteria.AddCondition("isdisabled", ConditionOperator.Equal, false);
                    memberQuery.AddOrder("fullname", OrderType.Ascending);

                    LinkEntity linkEntity = memberQuery.AddLink("teammembership", "systemuserid", "systemuserid");
                    linkEntity.LinkCriteria.AddCondition("teamid", ConditionOperator.Equal, selectedTeamId);

                    args.Result = Service.RetrieveMultiple(memberQuery);
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    EntityCollection members = (EntityCollection)args.Result;

                    listViewUsers.Items.Clear();

                    foreach (Entity user in members.Entities)
                    {
                        string userName = user.GetAttributeValue<string>("fullname");
                        string email = user.GetAttributeValue<string>("internalemailaddress");

                        ListViewItem item = new ListViewItem(userName);
                        item.SubItems.Add(email ?? "");
                        item.Tag = user.Id;
                        listViewUsers.Items.Add(item);
                    }

                    lblUsers.Text = "Team Members (" + members.Entities.Count + ") - " + selectedTeamName;
                }
            });
        }

        // ========== GET TEAM TYPE NAME ==========
        private string GetTeamTypeName(OptionSetValue teamType)
        {
            if (teamType == null)
                return "Unknown";

            switch (teamType.Value)
            {
                case 0: return "Owner";
                case 1: return "Access";
                case 2: return "AAD Security Group";
                case 3: return "AAD Office Group";
                default: return "Other";
            }
        }

        // ========== EXPORT ONLY SELECTED BU + SELECTED TEAM + USERS ==========
        private void btnExport_Click(object sender, EventArgs e)
        {
            // Validate: Must have selected a BU
            if (listViewBU.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a Business Unit first.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate: Must have users showing
            if (listViewUsers.Items.Count == 0)
            {
                MessageBox.Show("No users to export. Select a BU or Team first.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get selected BU name
            string selectedBUName = listViewBU.SelectedItems[0].Text;

            // Get selected Team name (if any)
            string selectedTeamName = "";
            string selectedTeamType = "";
            if (listViewTeams.SelectedItems.Count > 0)
            {
                selectedTeamName = listViewTeams.SelectedItems[0].Text;
                selectedTeamType = listViewTeams.SelectedItems[0].SubItems.Count > 1
                    ? listViewTeams.SelectedItems[0].SubItems[1].Text
                    : "";
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "CSV Files (*.csv)|*.csv";
            saveDialog.FileName = "BU_Hierarchy_" + selectedBUName.Replace(" ", "_") + ".csv";
            saveDialog.Title = "Export Selected Hierarchy";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Business Unit,Team Name,Team Type,User Name,Email");

                    // Export each user currently shown in the Users panel
                    foreach (ListViewItem userItem in listViewUsers.Items)
                    {
                        string userName = userItem.Text;
                        string email = userItem.SubItems.Count > 1 ? userItem.SubItems[1].Text : "";

                        sb.AppendLine(
                            EscapeCsv(selectedBUName) + "," +
                            EscapeCsv(selectedTeamName) + "," +
                            EscapeCsv(selectedTeamType) + "," +
                            EscapeCsv(userName) + "," +
                            EscapeCsv(email)
                        );
                    }

                    File.WriteAllText(saveDialog.FileName, sb.ToString());

                    MessageBox.Show(
                        "Export completed successfully!\n\n" +
                        "Business Unit: " + selectedBUName + "\n" +
                        (string.IsNullOrEmpty(selectedTeamName) ? "All BU Users" : "Team: " + selectedTeamName) + "\n" +
                        "Users Exported: " + listViewUsers.Items.Count + "\n\n" +
                        "File: " + saveDialog.FileName,
                        "Export Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LogInfo("Exported to: " + saveDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Export failed: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ========== CSV ESCAPE HELPER ==========
        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
        }
    }
}
