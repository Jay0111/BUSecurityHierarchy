# 🏢 BU Security Hierarchy Viewer

An **XrmToolBox** plugin to visualize the **Dynamics 365 / Dataverse** Business Unit → Team → User security hierarchy in an interactive tree view with **Excel export** capabilities.

---

## ✨ Features

| Feature | Description |
|---------|-------------|
| 🌳 **BU Tree View** | Hierarchical tree of all Business Units (parent-child) |
| 👥 **Teams List** | View all Teams under a selected Business Unit |
| 👤 **Users List** | View all Users within a selected Team |
| ➕ **Expand / Collapse** | Expand or collapse all BU nodes with one click|
| 📥 **Export** | Export teams & users to Excel with formatted worksheets |

---

## 📸 How It Works

```
┌─────────────────────────────────────────────────────────────┐
│  BU Security Hierarchy Viewer                                            │
│  [🔄 Load BU Hierarchy]  [➕ Expand All]  [➖ Collapse All] [📥 Export]│
├──────────────┬──────────────────┬───────────────────────────┤
│ 📂 Business  │ 👥 Teams         │ 👤 Users                  │
│    Units     │                  │                           │
│              │                  │                           │
│ 📁 Contoso   │ Team Name | Type │ User Name   | Email       │
│  ├📁 Sales   │ Sales Team|Owner │ John Smith  | john@co..   │
│  ├📁 Support │ West Team |Access│ Jane Doe    | jane@co..   │
│  └📁 HR      │           |      │             |             │
│              │                  │                           │
│ Select a BU  │ Select a Team    │ Users shown here          │
│ to see Teams │ to see Users     │                           │
└──────────────┴──────────────────┴───────────────────────────┘
```

**Flow:** Select a Business Unit → See its Teams and Users → Select a Team → See its Users

---

  ## 📥 Export Capabilities

  ### 🎯 Export Selected Team
  Click **Export** with a **team selected** to export:
  - **Single Excel file** with team details and all users
  - Team information (name, type)
  - All users in the selected team with email addresses

  **Excel Structure:**
  - Team metadata header
  - User list with name and email

  ### 🎯 Export All Teams & Users (Business Unit)
  Click **Export** with **only a BU selected** (no team selected) to export:
  - **Multi-sheet Excel workbook** with complete BU data

  **Excel Structure:**
  - **Sheet 1 - Summary:** BU name, export date, team count, user count
  - **Sheet 2 - Teams:** All teams with name and type
  - **Sheet 3 - Users:** All users with name and email

  > 💡 **Tip:** The export automatically generates timestamped filenames like `SalesBU_AllTeams_20260519_143022.xlsx`

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

  ### View Hierarchy
  1. Open **XrmToolBox**
  2. **Connect** to your Dynamics 365 / Dataverse environment
  3. Open **BU Security Hierarchy Viewer** from the tool list
  4. Click **🔄 Load BU Hierarchy**
  5. Click on any **Business Unit** in the tree to view its Teams and Users
  6. Click on any **Team** to view its Users
  7. Use **Expand All / Collapse All** to navigate the tree quickly

  ### Export Data
  **Option 1: Export Single Team**
  1. Select a **Business Unit** from the tree
  2. Select a **Team** from the list
  3. Click **📥 Export**
  4. Choose save location → Excel file created with team users

  **Option 2: Export All Teams & Users**
  1. Select a **Business Unit** from the tree (don't select a team)
  2. Click **📥 Export**
  3. Choose save location → Excel file created with 3 sheets (Summary, Teams, Users)

  ---

  ## 🔐 Required Security Privileges

  Your connected Dynamics 365 user must have the following **minimum privileges**:

  | Entity | Privilege | Recommended Scope |
  |--------|-----------|-------------------|
  | Business Unit | Read | Organization |
  | Team | Read | Organization |
  | User (SystemUser) | Read | Organization |

  ### How to Configure

  Dynamics 365 → Settings → Security → Security Roles → [Your Role]
    → "Business Management" tab
      → Business Unit → Read → Organization level
      → Team → Read → Organization level
      → User → Read → Organization level

  > ⚠️ If you don't have sufficient privileges, the plugin will show a friendly error message explaining which
  permissions are missing.

  ---

  ## 🛡️ Privacy & Security

  | Aspect | Details |
  |--------|---------|
  | ✅ **Local Execution** | Plugin runs entirely on your local machine |
  | ✅ **No External Calls** | No data is sent to any external server |
  | ✅ **Org Scoped** | Only reads data from YOUR connected Dynamics 365 org |
  | ✅ **Open Source** | Source code is fully open and auditable |
  | ✅ **Local Export** | Excel files saved only to locations you choose |
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
          ├── MyPluginControl.cs                # Main logic + Export
          ├── MyPluginControl.designer.cs       # UI layout
          │
          └── Resources/
              └── icon.png                      # Plugin icon (32x32)

  🗺️ Roadmap

  - Business Unit tree view
  - Teams list per BU
  - Users list per Team
  - Expand / Collapse all nodes
  - 📥 Export to Excel (selected team or all teams/users)
  - 🔍 Search / Filter Business Units
  - 🛡️ Security Roles per Team
  - 📊 User count summary per BU
  - 🌙 Dark mode support
  - 📥 Export team-user mappings (cross-reference)

  ---
  🐛 Troubleshooting

  ┌────────────────────────────────────────┬────────────────────────────────────────────────────────────────────────┐
  │                 Issue                  │                                Solution                                │
  ├────────────────────────────────────────┼────────────────────────────────────────────────────────────────────────┤
  │ "Please connect to an organization     │ Click the Connect button in XrmToolBox toolbar                         │
  │ first"                                 │                                                                        │
  ├────────────────────────────────────────┼────────────────────────────────────────────────────────────────────────┤
  │ "Access Denied" or                     │ Your security role lacks Read privilege on BU/Team/User. Contact your  │
  │ "SecLib::AccessCheckEx"                │ admin                                                                  │
  ├────────────────────────────────────────┼────────────────────────────────────────────────────────────────────────┤
  │ "Authentication Expired"               │ Reconnect to the organization                                          │
  ├────────────────────────────────────────┼────────────────────────────────────────────────────────────────────────┤
  │ No Business Units shown                │ Your security role may restrict BU visibility. Request                 │
  │                                        │ Organization-level Read                                                │
  ├────────────────────────────────────────┼────────────────────────────────────────────────────────────────────────┤
  │ Plugin not appearing in XrmToolBox     │ Ensure DLL is in the Plugins folder and restart XrmToolBox             │
  ├────────────────────────────────────────┼────────────────────────────────────────────────────────────────────────┤
  │ Export button does nothing             │ Ensure you've selected either a BU (for all teams) or a team (for      │
  │                                        │ single team export)                                                    │
  ├────────────────────────────────────────┼────────────────────────────────────────────────────────────────────────┤
  │ Excel file won't open                  │ Ensure you have Excel 2007+ or compatible software (LibreOffice,       │
  │                                        │ Google Sheets)                                                         │
  └────────────────────────────────────────┴────────────────────────────────────────────────────────────────────────┘

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

  - GitHub: https://github.com/Jay0111

  ---
  ⭐ Support

  If you find this plugin useful, please consider:

  - ⭐ Starring this repository on GitHub
  - 🐛 Reporting bugs via https://github.com/Jay0111/BUSecurityHierarchy/issues
  - 💡 Suggesting features via https://github.com/Jay0111/BUSecurityHierarchy/issues
  - 📣 Sharing with the Dynamics 365 community

  ---
  📋 Version History

  v1.2026.5.2 - May 2026
  - ✨ Added Excel export for selected team with users
  - ✨ Added Excel export for all teams and users in a BU
  - 📊 Multi-sheet workbook with Summary, Teams, and Users
  - 🎨 Formatted Excel output with headers and styling

  v1.2026.5.1 - May 2026
  - 🎉 Initial release
  - 🌳 Business Unit hierarchy tree view
  - 👥 Teams list per BU
  - 👤 Users list per team
  - ➕ Expand/Collapse controls

  ## Key Changes:

  1. **Updated subtitle** - mentions Excel export
  2. **Features table** - Changed "coming soon" to actual feature
  3. **New Export Capabilities section** - Detailed explanation of both export modes
  4. **Updated "How It Works"** - Added Export button to diagram
  5. **New Usage section** - Step-by-step export instructions
  6. **Updated Privacy section** - Added export-related privacy notes
  7. **Updated Roadmap** - Marked export as complete ✅
  8. **New troubleshooting entries** - Export-specific issues
  9. **New Version History section** - Documents your releases

  This clearly communicates your new export feature to users! 🎉
