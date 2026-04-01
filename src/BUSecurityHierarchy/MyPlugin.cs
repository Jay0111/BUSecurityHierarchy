using System.ComponentModel.Composition;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace BUSecurityHierarchy
{
    [Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "BU Security Hierarchy"),
        ExportMetadata("Description", "Visualize Dynamics 365 Business Unit → Team → User hierarchy in a tree structure"),
        ExportMetadata("SmallImageBase64", null),
        ExportMetadata("BigImageBase64", null),
        ExportMetadata("BackgroundColor", "Lavender"),
        ExportMetadata("PrimaryFontColor", "Black"),
        ExportMetadata("SecondaryFontColor", "Gray")]
    public class MyPlugin : PluginBase
    {
        public override IXrmToolBoxPluginControl GetControl()
        {
            return new MyPluginControl();
        }
    }
}
