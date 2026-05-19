# 🏢 BU Security Hierarchy Viewer

An **XrmToolBox** plugin to visualize the **Dynamics 365 / Dataverse** Business Unit → Team → User security hierarchy in an interactive tree view.

---

## ✨ Features

| Feature | Description |
|---------|-------------|
| 🌳 **BU Tree View** | Hierarchical tree of all Business Units (parent-child) |
| 👥 **Teams List** | View all Teams under a selected Business Unit |
| 👤 **Users List** | View all Users within a selected Team |
| ➕ **Expand / Collapse** | Expand or collapse all BU nodes with one click |
| 📥 **Export** | Export hierarchy data *(coming soon)* |

---

## 📸 How It Works

```
┌─────────────────────────────────────────────────────────────┐
│  BU Security Hierarchy Viewer                               │
│  [🔄 Load BU Hierarchy]  [➕ Expand All]  [➖ Collapse All] │
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

**Flow:** Select a Business Unit → See its Teams → Select a Team → See its Users

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

1. Open **XrmToolBox**
2. **Connect** to your Dynamics 365 / Dataverse environment
3. Open **BU Security Hierarchy Viewer** from the tool list
4. Click **🔄 Load BU Hierarchy**
5. Click on any **Business Unit** in the tree to view its Teams
6. Click on any **Team** to view its Users
7. Use **Expand All / Collapse All** to navigate the tree quickly

---

## 🔐 Required Security Privileges

Your connected Dynamics 365 user must have the following **minimum privileges**:

| Entity | Privilege | Recommended Scope |
|--------|-----------|-------------------|
| Business Unit | Read | Organization |
| Team | Read | Organization |
| User (SystemUser) | Read | Organization |

### How to Configure

```
Dynamics 365 → Settings → Security → Security Roles → [Your Role]
  → "Business Management" tab
    → Business Unit → Read → Organization level
    → Team → Read → Organization level
    → User → Read → Organization level
```

> ⚠️ If you don't have sufficient privileges, the plugin will show a friendly error message explaining which permissions are missing.

---

## 🛡️ Privacy & Security

| Aspect | Details |
|--------|---------|
| ✅ **Local Execution** | Plugin runs entirely on your local machine |
| ✅ **No External Calls** | No data is sent to any external server |
| ✅ **Org Scoped** | Only reads data from YOUR connected Dynamics 365 org |
| ✅ **Open Source** | Source code is fully open and auditable |
| ❌ **No Telemetry** | No tracking, analytics, or usage data collected |
| ❌ **No Data Storage** | Nothing is saved to disk unless you explicitly export |

---

## 🛠️ Build from Source

### Prerequisites

- Visual Studio 2019 or later
- .NET Framework 4.8
- XrmToolBox SDK (NuGet)

```bash
git clone https://github.com/Jay0111/BUSecurityHierarchy.git
```

1. Open `BUSecurityHierarchy.sln` in **Visual Studio 2022**
2. Set configuration to **Release**
3. Build → **Rebuild Solution**

Output DLL will be in `bin/Release/BUSecurityHierarchy.dll`

> ⚠️ **Note:** This is a .NET Framework 4.8 project. Use **Visual Studio** to build — `dotnet build` is not supported for .NET Framework projects.

### Test Locally in XrmToolBox

1. Build the project
2. Copy `BUSecurityHierarchy.dll` to your XrmToolBox **Plugins** folder:
   ```
   C:\Users\<you>\AppData\Roaming\MscrmTools\XrmToolBox\Plugins\
   ```
3. Restart XrmToolBox
4. Plugin will appear in the tool list

---

## 📁 Project Structure

```
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
        ├── MyPluginControl.cs                # Main logic
        ├── MyPluginControl.designer.cs       # UI layout
        │
        └── Resources/
            └── icon.png                      # Plugin icon (32x32)
```

## 🗺️ Roadmap

- [x] Business Unit tree view
- [x] Teams list per BU
- [x] Users list per Team
- [x] Expand / Collapse all nodes
- [ ] 📥 Export to CSV / Excel
- [ ] 🔍 Search / Filter Business Units
- [ ] 🛡️ Security Roles per Team
- [ ] 📊 User count summary per BU
- [ ] 🌙 Dark mode support

---

## 🐛 Troubleshooting

| Issue | Solution |
|-------|----------|
| **"Please connect to an organization first"** | Click the **Connect** button in XrmToolBox toolbar |
| **"Access Denied" or "SecLib::AccessCheckEx"** | Your security role lacks Read privilege on BU/Team/User. Contact your admin |
| **"Authentication Expired"** | Reconnect to the organization |
| **No Business Units shown** | Your security role may restrict BU visibility. Request Organization-level Read |
| **Plugin not appearing in XrmToolBox** | Ensure DLL is in the Plugins folder and restart XrmToolBox |

---

## 🤝 Contributing

Contributions are welcome!

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Commit** your changes (`git commit -m 'Add amazing feature'`)
4. **Push** to the branch (`git push origin feature/amazing-feature`)
5. Open a **Pull Request**

---

## 📄 License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

---

## 👨‍💻 Author

**Thota Jayadev**

- GitHub: [@Jay0111](https://github.com/Jay0111)

---

## ⭐ Support

If you find this plugin useful, please consider:

- ⭐ **Starring** this repository on GitHub
- 🐛 **Reporting bugs** via [GitHub Issues](https://github.com/Jay0111/BUSecurityHierarchy/issues)
- 💡 **Suggesting features** via [GitHub Issues](https://github.com/Jay0111/BUSecurityHierarchy/issues)
