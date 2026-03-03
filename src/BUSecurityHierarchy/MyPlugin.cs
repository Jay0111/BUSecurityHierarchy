using System.ComponentModel.Composition;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace BUSecurityHierarchy
{
    [Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "BU Security Hierarchy"),
        ExportMetadata("Description", "Generates Business units Hierarchy like BU --> Teams --> Users from Dynamics"),
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
