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
            this.btnExport = new System.Windows.Forms.ToolStripButton();
            this.btnExpandAll = new System.Windows.Forms.ToolStripButton();
            this.btnCollapseAll = new System.Windows.Forms.ToolStripButton();
            this.mainTableLayout = new System.Windows.Forms.TableLayoutPanel();

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

            this.toolStripMenu.SuspendLayout();
            this.mainTableLayout.SuspendLayout();
            this.panelBU.SuspendLayout();
            this.panelTeams.SuspendLayout();
            this.panelUsers.SuspendLayout();
            this.SuspendLayout();

            // ========== ToolStrip ==========
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.btnLoadHierarchy,
                this.btnExport,
                new System.Windows.Forms.ToolStripSeparator(),
                this.btnExpandAll,
                this.btnCollapseAll
            });
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(1000, 25);
            this.toolStripMenu.TabIndex = 0;

            // btnLoadHierarchy
            this.btnLoadHierarchy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnLoadHierarchy.Name = "btnLoadHierarchy";
            this.btnLoadHierarchy.Text = "🔄 Load BU Hierarchy";
            this.btnLoadHierarchy.Click += new System.EventHandler(this.btnLoadHierarchy_Click);

            // btnExport
            this.btnExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExport.Name = "btnExport";
            this.btnExport.Text = "📥 Export";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);

            // btnExpandAll
            this.btnExpandAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExpandAll.Name = "btnExpandAll";
            this.btnExpandAll.Text = "➕ Expand All";
            this.btnExpandAll.Click += new System.EventHandler(this.btnExpandAll_Click);

            // btnCollapseAll
            this.btnCollapseAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnCollapseAll.Name = "btnCollapseAll";
            this.btnCollapseAll.Text = "➖ Collapse All";
            this.btnCollapseAll.Click += new System.EventHandler(this.btnCollapseAll_Click);

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
            this.mainTableLayout.Controls.Add(this.panelUsers, 2, 0);

            // ========== Left Panel - Business Units (TreeView) ==========
            this.lblBU.Text = "📂 Business Units";
            this.lblBU.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblBU.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblBU.Height = 30;
            this.lblBU.BackColor = System.Drawing.Color.LavenderBlush;
            this.lblBU.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBU.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lblBU.Name = "lblBU";

            this.treeViewBU.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewBU.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.treeViewBU.ItemHeight = 24;
            this.treeViewBU.ShowLines = true;
            this.treeViewBU.ShowPlusMinus = true;
            this.treeViewBU.ShowRootLines = true;
            this.treeViewBU.HideSelection = false;
            this.treeViewBU.Name = "treeViewBU";
            this.treeViewBU.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewBU_AfterSelect);

            this.panelBU.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBU.Margin = new System.Windows.Forms.Padding(3);
            this.panelBU.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelBU.Name = "panelBU";
            this.panelBU.Controls.Add(this.treeViewBU);
            this.panelBU.Controls.Add(this.lblBU);

            // ========== Middle Panel - Teams ==========
            this.lblTeams.Text = "👥 Teams";
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
            this.listViewTeams.Columns.Add("Team Name", 200);
            this.listViewTeams.Columns.Add("Type", 100);
            this.listViewTeams.SelectedIndexChanged += new System.EventHandler(this.listViewTeams_SelectedIndexChanged);

            this.panelTeams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTeams.Margin = new System.Windows.Forms.Padding(3);
            this.panelTeams.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTeams.Name = "panelTeams";
            this.panelTeams.Controls.Add(this.listViewTeams);
            this.panelTeams.Controls.Add(this.lblTeams);

            // ========== Right Panel - Users ==========
            this.lblUsers.Text = "👤 Users";
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
            this.listViewUsers.Columns.Add("User Name", 200);
            this.listViewUsers.Columns.Add("Email", 200);

            this.panelUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelUsers.Margin = new System.Windows.Forms.Padding(3);
            this.panelUsers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelUsers.Name = "panelUsers";
            this.panelUsers.Controls.Add(this.listViewUsers);
            this.panelUsers.Controls.Add(this.lblUsers);

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
            this.mainTableLayout.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton btnLoadHierarchy;
        private System.Windows.Forms.ToolStripButton btnExport;
        private System.Windows.Forms.ToolStripButton btnExpandAll;
        private System.Windows.Forms.ToolStripButton btnCollapseAll;
        private System.Windows.Forms.TableLayoutPanel mainTableLayout;

        // Left Panel
        private System.Windows.Forms.Panel panelBU;
        private System.Windows.Forms.Label lblBU;
        private System.Windows.Forms.TreeView treeViewBU;

        // Middle Panel
        private System.Windows.Forms.Panel panelTeams;
        private System.Windows.Forms.Label lblTeams;
        private System.Windows.Forms.ListView listViewTeams;

        // Right Panel
        private System.Windows.Forms.Panel panelUsers;
        private System.Windows.Forms.Label lblUsers;
        private System.Windows.Forms.ListView listViewUsers;
    }
}
