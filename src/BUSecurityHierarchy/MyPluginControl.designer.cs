using System;

namespace BUSecurityHierarchy
{
    partial class MyPluginControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.btnLoadHierarchy = new System.Windows.Forms.ToolStripButton();
            this.btnExpandAll = new System.Windows.Forms.ToolStripButton();
            this.btnCollapseAll = new System.Windows.Forms.ToolStripButton();
            this.btnExport = new System.Windows.Forms.ToolStripButton();
            this.mainTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.rightPanelLayout = new System.Windows.Forms.TableLayoutPanel();

            // Left Panel - Business Units (TreeView)
            this.panelBU = new System.Windows.Forms.Panel();
            this.lblBU = new System.Windows.Forms.Label();
            this.treeViewBU = new System.Windows.Forms.TreeView();

            // Middle Panel - Teams
            this.panelTeams = new System.Windows.Forms.Panel();
            this.lblTeams = new System.Windows.Forms.Label();
            this.listViewTeams = new System.Windows.Forms.ListView();

            // Right Panel - Users
            this.panelUsers = new System.Windows.Forms.Panel();
            this.lblUsers = new System.Windows.Forms.Label();
            this.listViewUsers = new System.Windows.Forms.ListView();

            //Right Panel Below - Security Roles
            this.panelRoles = new System.Windows.Forms.Panel();
            this.lblRoles = new System.Windows.Forms.Label();
            this.lblRoleChangesWarning = new System.Windows.Forms.Label();
            this.btnSaveRoles = new System.Windows.Forms.Button();
            this.chkListRoles = new System.Windows.Forms.CheckedListBox();

            this.toolStripMenu.SuspendLayout();
            this.mainTableLayout.SuspendLayout();
            this.rightPanelLayout.SuspendLayout();
            this.panelBU.SuspendLayout();
            this.panelTeams.SuspendLayout();
            this.panelUsers.SuspendLayout();
            this.panelRoles.SuspendLayout();
            this.SuspendLayout();

            // ========== ToolStrip ==========
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.btnLoadHierarchy,
                this.btnExpandAll,
                this.btnCollapseAll,
                this.btnExport
            });
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(1000, 25);
            this.toolStripMenu.TabIndex = 0;

            // btnLoadHierarchy
            this.btnLoadHierarchy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnLoadHierarchy.Name = "btnLoadHierarchy";
            this.btnLoadHierarchy.Text = "Load BU Hierarchy";
            this.btnLoadHierarchy.Click += new System.EventHandler(this.btnLoadHierarchy_Click);

            // btnExpandAll
            this.btnExpandAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExpandAll.Name = "btnExpandAll";
            this.btnExpandAll.Text = "Expand All";
            this.btnExpandAll.Click += new System.EventHandler(this.btnExpandAll_Click);

            // btnCollapseAll
            this.btnCollapseAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnCollapseAll.Name = "btnCollapseAll";
            this.btnCollapseAll.Text = "Collapse All";
            this.btnCollapseAll.Click += new System.EventHandler(this.btnCollapseAll_Click);

            // btnExport
            this.btnExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExport.Name = "btnExport";
            this.btnExport.Text = "Export";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);

            // ========== Main TableLayoutPanel (3 columns) ==========
            this.mainTableLayout.ColumnCount = 3;
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.mainTableLayout.RowCount = 1;
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayout.Location = new System.Drawing.Point(0, 25);
            this.mainTableLayout.Name = "mainTableLayout";
            this.mainTableLayout.Padding = new System.Windows.Forms.Padding(5);
            this.mainTableLayout.Controls.Add(this.panelBU, 0, 0);
            this.mainTableLayout.Controls.Add(this.panelTeams, 1, 0);
            this.mainTableLayout.Controls.Add(this.rightPanelLayout, 2, 0);

            //// ========== Right Panel Layout (Nested - 2 rows) ==========
            this.rightPanelLayout.ColumnCount = 1;
            this.rightPanelLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rightPanelLayout.RowCount = 2;
            this.rightPanelLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 51F)); // For Users
            this.rightPanelLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 49F)); // For Roles
            this.rightPanelLayout.Margin = new System.Windows.Forms.Padding(0); 
            this.rightPanelLayout.Padding = new System.Windows.Forms.Padding(0);
            this.rightPanelLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightPanelLayout.Name = "rightPanelLayout";
            this.rightPanelLayout.Controls.Add(this.panelUsers, 0, 0);
            this.rightPanelLayout.Controls.Add(this.panelRoles, 0, 1);

            // ========== Left Panel - Business Units (TreeView) ==========
            this.lblBU.Text = "Business Units";
            this.lblBU.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblBU.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblBU.Height = 30;
            this.lblBU.BackColor = System.Drawing.Color.LavenderBlush;
            this.lblBU.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBU.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lblBU.Name = "lblBU";

            this.treeViewBU.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewBU.Name = "treeViewBU";
            this.treeViewBU.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.treeViewBU.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewBU_AfterSelect);

            this.panelBU.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBU.Margin = new System.Windows.Forms.Padding(3);
            this.panelBU.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelBU.Name = "panelBU";
            this.panelBU.Controls.Add(this.treeViewBU);
            this.panelBU.Controls.Add(this.lblBU);

            // ========== Middle Panel - Teams ==========
            this.lblTeams.Text = "Teams";
            this.lblTeams.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTeams.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTeams.Height = 30;
            this.lblTeams.BackColor = System.Drawing.Color.Lavender;
            this.lblTeams.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblTeams.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lblTeams.Name = "lblTeams";

            this.listViewTeams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewTeams.View = System.Windows.Forms.View.Details;
            this.listViewTeams.FullRowSelect = true;
            this.listViewTeams.GridLines = true;
            this.listViewTeams.Name = "listViewTeams";
            this.listViewTeams.Columns.Add("Team Name", 230);
            this.listViewTeams.Columns.Add("Type", 200);
            this.listViewTeams.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewTeams.SelectedIndexChanged += new System.EventHandler(this.listViewTeams_SelectedIndexChanged);

            this.panelTeams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTeams.Margin = new System.Windows.Forms.Padding(3);
            this.panelTeams.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTeams.Name = "panelTeams";
            this.panelTeams.Controls.Add(this.listViewTeams);
            this.panelTeams.Controls.Add(this.lblTeams);

            // ========== Right Panel - Users ==========
            this.lblUsers.Text = "Users";
            this.lblUsers.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblUsers.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblUsers.Height = 30;
            this.lblUsers.BackColor = System.Drawing.Color.Honeydew;
            this.lblUsers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblUsers.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lblUsers.Name = "lblUsers";

            this.listViewUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewUsers.View = System.Windows.Forms.View.Details;
            this.listViewUsers.FullRowSelect = true;
            this.listViewUsers.GridLines = true;
            this.listViewUsers.Name = "listViewUsers";
            this.listViewUsers.Columns.Add("User Name", 230);
            this.listViewUsers.Columns.Add("Email", 200);

            this.panelUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelUsers.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.panelUsers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelUsers.Name = "panelUsers";
            this.panelUsers.Controls.Add(this.listViewUsers);
            this.panelUsers.Controls.Add(this.lblUsers);

            // ==========  Right Panel Bottom - Security Roles ==========
            this.lblRoles.Text = "🛡️ Security Roles";
            this.lblRoles.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblRoles.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblRoles.Height = 30;
            this.lblRoles.BackColor = System.Drawing.Color.LightSteelBlue;
            this.lblRoles.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblRoles.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lblRoles.Margin = new System.Windows.Forms.Padding(0);
            this.lblRoles.Name = "lblRoles";

            // SEARCH TEXTBOX
            this.txtSearchRoles = new System.Windows.Forms.TextBox();
            this.txtSearchRoles.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtSearchRoles.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtSearchRoles.Name = "txtSearchRoles";
            this.txtSearchRoles.Height = 25;
            this.txtSearchRoles.ForeColor = System.Drawing.Color.Gray;
            this.txtSearchRoles.Text = "Type to search roles...";
            this.txtSearchRoles.TextChanged += new System.EventHandler(this.txtSearchRoles_TextChanged);
            this.txtSearchRoles.Enter += new System.EventHandler(this.txtSearchRoles_Enter);
            this.txtSearchRoles.Leave += new System.EventHandler(this.txtSearchRoles_Leave);

            //Warning Message 
            this.lblRoleChangesWarning = new System.Windows.Forms.Label();
            this.lblRoleChangesWarning.Text = "⚠️ You have unsaved changes. Click Save to apply or cancel to discard.";
            this.lblRoleChangesWarning.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblRoleChangesWarning.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblRoleChangesWarning.Height = 25;
            this.lblRoleChangesWarning.BackColor = System.Drawing.Color.Yellow;
            this.lblRoleChangesWarning.ForeColor = System.Drawing.Color.DarkRed;
            this.lblRoleChangesWarning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblRoleChangesWarning.Visible = false; // Hidden by default
            this.lblRoleChangesWarning.Name = "lblRoleChangesWarning";

            this.chkListRoles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkListRoles.Name = "chkListRoles";
            this.chkListRoles.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkListRoles.BorderStyle = System.Windows.Forms.BorderStyle.None; 
            this.chkListRoles.Margin = new System.Windows.Forms.Padding(0); 
            this.chkListRoles.IntegralHeight = false;
            this.chkListRoles.CheckOnClick = true;
            this.chkListRoles.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chkListRoles_ItemCheck);

            this.btnSaveRoles = new System.Windows.Forms.Button();
            this.btnSaveRoles.Text = "💾 Save Role Changes";
            this.btnSaveRoles.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnSaveRoles.Height = 35;
            this.btnSaveRoles.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSaveRoles.BackColor = System.Drawing.Color.ForestGreen;
            this.btnSaveRoles.ForeColor = System.Drawing.Color.White;
            this.btnSaveRoles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveRoles.Visible = false; // Hidden by default
            this.btnSaveRoles.Name = "btnSaveRoles";
            this.btnSaveRoles.Click += new System.EventHandler(this.btnSaveRoles_Click);

            this.panelRoles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRoles.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.panelRoles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelRoles.Name = "panelRoles";
            this.panelRoles.Controls.Add(this.chkListRoles);
            this.panelRoles.Controls.Add(this.lblRoleChangesWarning);
            this.panelRoles.Controls.Add(this.txtSearchRoles);
            this.panelRoles.Controls.Add(this.btnSaveRoles);
            this.panelRoles.Controls.Add(this.lblRoles);

            // ========== MyPluginControl ==========
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainTableLayout);
            this.Controls.Add(this.toolStripMenu);
            this.Name = "MyPluginControl";
            this.Size = new System.Drawing.Size(1000, 600);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);

            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.panelBU.ResumeLayout(false);
            this.panelTeams.ResumeLayout(false);
            this.panelUsers.ResumeLayout(false);
            this.panelRoles.ResumeLayout(false); 
            this.rightPanelLayout.ResumeLayout(false);
            this.mainTableLayout.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton btnLoadHierarchy;
        private System.Windows.Forms.ToolStripButton btnExpandAll;
        private System.Windows.Forms.ToolStripButton btnCollapseAll;
        private System.Windows.Forms.ToolStripButton btnExport;
        private System.Windows.Forms.TableLayoutPanel mainTableLayout;
        private System.Windows.Forms.TableLayoutPanel rightPanelLayout;

        // Add these fields in the Designer.cs class
        private System.Windows.Forms.Button btnSaveRoles;
        private System.Windows.Forms.Label lblRoleChangesWarning;
        private Guid? _selectedTeamId; // Track selected team
        private bool _hasUnsavedChanges; // Track unsaved changes
        

        // Left Panel
        private System.Windows.Forms.Panel panelBU;
        private System.Windows.Forms.Label lblBU;
        private System.Windows.Forms.TreeView treeViewBU;

        // Middle Panel
        private System.Windows.Forms.Panel panelTeams;
        private System.Windows.Forms.Label lblTeams;
        private System.Windows.Forms.ListView listViewTeams;

        // Right Panel 1
        private System.Windows.Forms.Panel panelUsers;
        private System.Windows.Forms.Label lblUsers;
        private System.Windows.Forms.ListView listViewUsers;
        //Right Panel 2
        private System.Windows.Forms.Panel panelRoles;
        private System.Windows.Forms.Label lblRoles;
        private System.Windows.Forms.CheckedListBox chkListRoles;
        //Search Roles
        private System.Windows.Forms.TextBox txtSearchRoles;
        private System.Collections.Generic.List<RoleItem> _allRoles; // Store all roles for search functionality
    }
}
