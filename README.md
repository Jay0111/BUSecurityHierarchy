 # 🏢 BU Security Hierarchy Viewer

  An **XrmToolBox** plugin to visualize the **Dynamics 365 / Dataverse** Business Unit → Team → User → **Security
  Roles** hierarchy in an interactive view with **Excel export** capabilities and **role assignment/removal**
  functionality.

  ---

  ## ✨ Features

  | Feature | Description |
  |---------|-------------|
  | 🌳 **BU Tree View** | Hierarchical tree of all Business Units (parent-child) |
  | 👥 **Teams List** | View all Teams under a selected Business Unit |
  | 👤 **Users List** | View all Users within a selected Team |
  | 🛡️ **Security Roles** | View and manage security roles assigned to teams with checkboxes |
  | ✅ **Role Assignment** | Check/uncheck roles to assign or remove from selected team |
  | 💾 **Save Changes** | Smart save button appears only when role changes are detected |
  | ➕ **Expand / Collapse** | Expand or collapse all BU nodes with one click |
  | 📥 **Export to Excel** | Export teams, users, AND assigned roles to formatted Excel worksheets |
  | 💡 **Quick Guide** | Collapsible info banner with usage guidelines |
  | 🔄 **Connection Aware** | Automatically resets when switching between environments |

  ---

  ## 📸 How It Works

  ┌───────────────────────────────────────────────────────────────────────────┐
  │  BU Security Hierarchy Viewer                                             │
  │  [🔄 Load BU Hierarchy] [➕ Expand All] [➖ Collapse All] [📥 Export]    │
  ├────────────────────────────────────────────────────────────────────────────┤
  │ 💡 Quick Guide: Export without team = all data | Select team = filtered   │
  │    export | Role changes = Save button appears                      [Hide]│
  ├──────────────┬──────────────────┬─────────────────────────────────────────┤
  │ 📂 Business  │ 👥 Teams         │ 👤 Users                  │ 🛡️ Roles    │
  │    Units     │                  │                           │             │
  │              │                  │                           │             │
  │ 📁 Contoso   │ Team Name | Type │ User Name   | Email       │ ☑ System    │
  │  ├📁 Sales   │ Sales Team|Owner │ John Smith  | john@co..   │   Admin     │
  │  ├📁 Support │ West Team |Access│ Jane Doe    | jane@co..   │ ☑ Salesperson│
  │  └📁 HR      │           |      │             |             │ ☐ Marketing │
  │              │                  │                           │   Manager   │
  │              │                  │                           │             │
  │              │                  │                           │ [💾 Save]   │
  └──────────────┴──────────────────┴───────────────────────────────────────────┘

  **Flow:**
  1. Select a Business Unit → See Teams, Users, and Roles
  2. Select a Team → See Team Members and Assigned Roles (checked)
  3. Check/Uncheck Roles → Save button appears
  4. Click Save → Roles assigned/removed from team
  5. Click Export → Get Excel with complete data

  ---

  ## 🛡️ Security Role Management

  ### View Assigned Roles
  - Select any **Team** to instantly see which roles are assigned
  - ✅ **Checked roles** = Currently assigned to the team
  - ☐ **Unchecked roles** = Available roles in the Business Unit

  ### Assign/Remove Roles
  1. **Select a team** from the Teams list
  2. **Check** a role to assign it to the team
  3. **Uncheck** a role to remove it from the team
  4. **Save button appears** automatically when changes are detected
  5. Click **💾 Save** to apply changes
  6. Changes are saved to Dynamics 365 in real-time

  ### Smart Save Detection
  - ✅ Save button **only appears** when you make changes
  - ✅ Save button **automatically hides** if you revert to original state
  - ✅ Yellow warning banner shows when unsaved changes exist
  - ✅ Confirmation dialog before applying changes

  ---

  ## 📥 Export Capabilities

  ### 🎯 Export Selected Team (with Assigned Roles)
  Click **Export** with a **team selected** to export:
  - **Single Excel file** with team details, users, and **assigned security roles**
  - Team information (name, type)
  - All users in the selected team with email addresses
  - **All security roles assigned to that team**

  **Excel Structure:**
  - Team metadata header (name, type, export date, counts)
  - User list with name and email
  - **Security Roles list with role name and business unit**

  ### 🎯 Export All Teams, Users & Roles (Business Unit)
  Click **Export** with **only a BU selected** (no team selected) to export:
  - **Multi-sheet Excel workbook** with complete BU data

  **Excel Structure:**
  - **Sheet 1 - Summary:** BU name, export date, team count, user count, role count
  - **Sheet 2 - Teams:** All teams with name and type
  - **Sheet 3 - Users:** All users with name and email
  - **Sheet 4 - Security Roles:** All roles in the BU with business unit reference

  > 💡 **Tip:** Export automatically generates timestamped filenames like `SalesBU_AllTeams_20260623_143022.xlsx`

  ---

  ## 💡 Quick Guide Banner

  The plugin includes a **collapsible info banner** at the top with helpful tips:

  **Guidelines shown:**
  - 📘 Export All: Click Export without selecting a team
  - 📘 Export Team: Select a team first for filtered export
  - 📘 Assign Roles: Check/uncheck roles, Save button appears automatically

  **Features:**
  - ✅ Collapsible - Click **Hide** to save screen space
  - ✅ Click **Show** to expand again
  - ✅ Persists across sessions
  - ✅ Clean, non-intrusive design

  ---

  ## 📦 Installation

  1. Open **XrmToolBox**
  2. Go to **Tool Library** (Plugin Store)
  3. Search for **"BU Security Hierarchy"**
  4. Click **Install**
  5. Restart XrmToolBox
  6. Find the plugin under **Tools**

  ---

  ## 🚀 Usage

  ### View Hierarchy & Roles
  1. Open **XrmToolBox**
  2. **Connect** to your Dynamics 365 / Dataverse environment
  3. Open **BU Security Hierarchy Viewer** from the tool list
  4. Click **🔄 Load BU Hierarchy**
  5. Click on any **Business Unit** in the tree to view its Teams, Users, and Roles
  6. Click on any **Team** to view its Users and Assigned Roles (checked)
  7. Use **Expand All / Collapse All** to navigate the tree quickly

  ### Assign/Remove Security Roles
  1. Select a **Business Unit** from the tree
  2. Select a **Team** from the list
  3. **Check** or **Uncheck** roles in the Roles panel
  4. **💾 Save** button appears automatically
  5. Click **Save** to apply changes
  6. Confirm the changes in the dialog
  7. Roles are updated in Dynamics 365

  ### Export Data
  **Option 1: Export Single Team (with Roles)**
  1. Select a **Business Unit** from the tree
  2. Select a **Team** from the list
  3. Click **📥 Export**
  4. Choose save location → Excel file created with team users and **assigned roles**

  **Option 2: Export All Teams, Users & Roles**
  1. Select a **Business Unit** from the tree (don't select a team)
  2. Click **📥 Export**
  3. Choose save location → Excel file created with 4 sheets (Summary, Teams, Users, **Security Roles**)

  ---

  ## 🔐 Required Security Privileges

  Your connected Dynamics 365 user must have the following **minimum privileges**:

  | Entity | Privilege | Recommended Scope | Required For |
  |--------|-----------|-------------------|--------------|
  | Business Unit | Read | Organization | View BU tree |
  | Team | Read | Organization | View teams |
  | User (SystemUser) | Read | Organization | View users |
  | **Role** | **Read** | **Organization** | **View security roles** |
  | **TeamRoles** | **Read, Append, Append To** | **Organization** | **Assign/remove roles** |

  ### How to Configure

  **Dynamics 365** → **Settings** → **Security** → **Security Roles** → **[Your Role]**
    → **"Business Management"** tab
      → Business Unit → Read → Organization level
      → Team → Read → Organization level
      → User → Read → Organization level
      → **Security Role → Read → Organization level**

    → **"Customization"** tab
      → **TeamRoles → Read, Append, Append To → Organization level**

  > ⚠️ If you don't have sufficient privileges, the plugin will show a friendly error message explaining which
  permissions are missing.

  ---

  ## 🛡️ Privacy & Security

  | Aspect | Details |
  |--------|---------|
  | ✅ **Local Execution** | Plugin runs entirely on your local machine |
  | ✅ **No External Calls** | No data is sent to any external server |
  | ✅ **Org Scoped** | Only reads/writes data in YOUR connected Dynamics 365 org |
  | ✅ **Open Source** | Source code is fully open and auditable |
  | ✅ **Local Export** | Excel files saved only to locations you choose |
  | ✅ **Role Assignment Logging** | All role changes are logged in Dynamics 365 audit trail |
  | ❌ **No Telemetry** | No tracking, analytics, or usage data collected |
  | ❌ **No Cloud Storage** | Exported data stays on your local machine |

  ---

  ## 🛠️ Build from Source

  ### Prerequisites

  - Visual Studio 2019 or later
  - .NET Framework 4.8
  - XrmToolBox SDK (NuGet)

  ```bash
  git clone https://github.com/Jay0111/BUSecurityHierarchy.git
  cd BUSecurityHierarchy

  1. Open BUSecurityHierarchy.sln in Visual Studio 2022
  2. Set configuration to Release
  3. Build → Rebuild Solution

  Output DLL will be in bin/Release/BUSecurityHierarchy.dll

  ▎ ⚠️ Note: This is a .NET Framework 4.8 project. Use Visual Studio to build — dotnet build is not supported for .NET
  ▎ Framework projects.

  Test Locally in XrmToolBox

  1. Build the project
  2. Copy BUSecurityHierarchy.dll to your XrmToolBox Plugins folder:
  C:\Users\<you>\AppData\Roaming\MscrmTools\XrmToolBox\Plugins\
  3. Restart XrmToolBox
  4. Plugin will appear in the tool list

  ---
  📁 Project Structure

  BUSecurityHierarchy/
  ├── .gitignore
  ├── LICENSE
  ├── README.md
  │
  └── src/
      └── BUSecurityHierarchy/
          ├── BUSecurityHierarchy.csproj        # Project file
          ├── BUSecurityHierarchy.nuspec        # NuGet package spec
          ├── MyPlugin.cs                       # Plugin registration
          ├── MyPluginControl.cs                # Main logic + Export + Role Assignment
          ├── MyPluginControl.designer.cs       # UI layout
          │
          └── Resources/
              └── icon.png                      # Plugin icon (32x32)

  ---
  🗺️ Roadmap

  - ✅ Business Unit tree view
  - ✅ Teams list per BU
  - ✅ Users list per Team
  - ✅ Expand / Collapse all nodes
  - ✅ Export to Excel (selected team or all teams/users)
  - ✅ Security Roles per Team
  - ✅ Assign/Remove Security Roles
  - ✅ Export with Security Roles
  - ✅ Collapsible Quick Guide Banner
  - ✅ Connection Change Detection
  - 🔜 Search / Filter Business Units
  - 🔜 User count summary per BU
  - 🔜 Dark mode support
  - 🔜 Bulk role assignment (multiple teams)
  - 🔜 Role comparison across teams
  - 🔜 Export team-user-role mapping (matrix view)

  ---
  🐛 Troubleshooting

  ┌─────────────────────────────────────────┬───────────────────────────────────────────────────────────────────────┐
  │                  Issue                  │                               Solution                                │
  ├─────────────────────────────────────────┼───────────────────────────────────────────────────────────────────────┤
  │ "Please connect to an organization      │ Click the Connect button in XrmToolBox toolbar                        │
  │ first"                                  │                                                                       │
  ├─────────────────────────────────────────┼───────────────────────────────────────────────────────────────────────┤
  │ "Access Denied" or                      │ Your security role lacks Read privilege on BU/Team/User/Role. Contact │
  │ "SecLib::AccessCheckEx"                 │  your admin                                                           │
  ├─────────────────────────────────────────┼───────────────────────────────────────────────────────────────────────┤
  │ "Authentication Expired"                │ Reconnect to the organization                                         │
  ├─────────────────────────────────────────┼───────────────────────────────────────────────────────────────────────┤
  │ No Business Units shown                 │ Your security role may restrict BU visibility. Request                │
  │                                         │ Organization-level Read                                               │
  ├─────────────────────────────────────────┼───────────────────────────────────────────────────────────────────────┤
  │ Plugin not appearing in XrmToolBox      │ Ensure DLL is in the Plugins folder and restart XrmToolBox            │
  ├─────────────────────────────────────────┼───────────────────────────────────────────────────────────────────────┤
  │ Export button does nothing              │ Ensure you've selected either a BU (for all) or a team (for single    │
  │                                         │ export)                                                               │
  ├─────────────────────────────────────────┼───────────────────────────────────────────────────────────────────────┤
  │ Excel file won't open                   │ Ensure you have Excel 2007+ or compatible software (LibreOffice,      │
  │                                         │ Google Sheets)                                                        │
  ├─────────────────────────────────────────┼───────────────────────────────────────────────────────────────────────┤
  │ Can't assign roles                      │ Missing Append/Append To privilege on TeamRoles entity                │
  ├─────────────────────────────────────────┼───────────────────────────────────────────────────────────────────────┤
  │ Save button not appearing               │ Select a team first, then check/uncheck roles                         │
  ├─────────────────────────────────────────┼───────────────────────────────────────────────────────────────────────┤
  │ Save button won't hide after reverting  │ Uncheck back to original state - button auto-hides                    │
  ├─────────────────────────────────────────┼───────────────────────────────────────────────────────────────────────┤
  │ Changes disappeared after connection    │ UI auto-resets when switching environments for data safety            │
  │ switch                                  │                                                                       │
  └─────────────────────────────────────────┴───────────────────────────────────────────────────────────────────────┘

  ---
  🤝 Contributing

  Contributions are welcome!

  1. Fork the repository
  2. Create a feature branch (git checkout -b feature/amazing-feature)
  3. Commit your changes (git commit -m 'Add amazing feature')
  4. Push to the branch (git push origin feature/amazing-feature)
  5. Open a Pull Request

  ---
  📄 License

  This project is licensed under the MIT License — see the LICENSE file for details.

  ---
  👨‍💻 Author

  Thota Jayadev

  - GitHub: @Jay0111 (https://github.com/Jay0111)
  - LinkedIn: Thota Jayadev (https://www.linkedin.com/in/thota-jayadev)

  ---
  ⭐ Support

  If you find this plugin useful, please consider:

  - ⭐ Starring this repository on GitHub
  - 🐛 Reporting bugs via Issues (https://github.com/Jay0111/BUSecurityHierarchy/issues)
  - 💡 Suggesting features via Issues (https://github.com/Jay0111/BUSecurityHierarchy/issues)
  - 📣 Sharing with the Dynamics 365 community

  ---
  📋 Version History

  v1.2026.6.3 - June 2026

  - ✨ Added Security Roles panel with checked list view
  - ✨ Role Assignment/Removal functionality with smart save detection
  - 💾 Save button appears only when changes detected
  - ⚠️ Warning banner for unsaved changes
  - 📥 Export now includes Security Roles (both single team and all export)
  - 💡 Collapsible Quick Guide banner for user guidance
  - 🔄 Connection change detection - auto-clears UI when switching environments
  - 🎨 Improved UI layout with right panel split (Users + Roles)
  - 🛡️ Enhanced security - validates privileges before role operations

  v1.2026.5.4 - May 2026

  - ✨ Added Excel export for selected team with users
  - ✨ Added Excel export for all teams and users in a BU
  - 📊 Multi-sheet workbook with Summary, Teams, and Users
  - 🎨 Formatted Excel output with headers and styling
  - 🔧 Timestamped export filenames

  v1.2026.5.1 - May 2026

  - 🎉 Initial release
  - 🌳 Business Unit hierarchy tree view
  - 👥 Teams list per BU
  - 👤 Users list per team
  - ➕ Expand/Collapse controls

  ---
  🙏 Acknowledgments

  - Built with XrmToolBox (https://www.xrmtoolbox.com/) framework
  - Excel export powered by ClosedXML (https://github.com/ClosedXML/ClosedXML)
  - Thanks to the Dynamics 365 community for feedback and feature requests

  ---
  📞 Get Help

  - 📖 Read the Documentation (https://github.com/Jay0111/BUSecurityHierarchy#readme)
  - 🐛 Report an Issue (https://github.com/Jay0111/BUSecurityHierarchy/issues/new)
  - 💬 Ask a Question (https://github.com/Jay0111/BUSecurityHierarchy/discussions)
  - 📧 Email: [jayadevthota23@gmail.com]

  