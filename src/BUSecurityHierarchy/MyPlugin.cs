using System.ComponentModel.Composition;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace BUSecurityHierarchy
{
    [Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "BU Security Hierarchy"),
        ExportMetadata("Description", "Visualize Dynamics 365 Business Unit → Team → User hierarchy in a tree structure"),
        ExportMetadata("SmallImageBase64", "\r\niVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAADWSURBVFhH7ZQxDoMwDEU5HbfpJbgAM5eAJRdA6sTCjJgRC1sP4KqlIgEnXyZFwGBLXvx/nCfLSZJmhq7MZFs4OxVAARTgrgAtNYRipDzgb6pwL65FA3zChUCXIE0AwA4VPQ1MA36oxQBkNZXTrA3PWuBHWgzAMoEXlYXADzUBQCjWzdAlSPsDYP8SuhPbAcAb2h2grhX4DwcwlFbjLE49Pb41C8X83p2xGQHgTGABMJR3v9rmkwrVxQAoVnDO/+AL+2QPAuCTCZ/zeyHAeakACqAACqAAb/QbwzHH9Yl/AAAAAElFTkSuQmCC"),
        ExportMetadata("BigImageBase64", "\r\niVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAADWSURBVFhH7ZQxDoMwDEU5HbfpJbgAM5eAJRdA6sTCjJgRC1sP4KqlIgEnXyZFwGBLXvx/nCfLSZJmhq7MZFs4OxVAARTgrgAtNYRipDzgb6pwL65FA3zChUCXIE0AwA4VPQ1MA36oxQBkNZXTrA3PWuBHWgzAMoEXlYXADzUBQCjWzdAlSPsDYP8SuhPbAcAb2h2grhX4DwcwlFbjLE49Pb41C8X83p2xGQHgTGABMJR3v9rmkwrVxQAoVnDO/+AL+2QPAuCTCZ/zeyHAeakACqAACqAAb/QbwzHH9Yl/AAAAAElFTkSuQmCC"),
        ExportMetadata("BackgroundColor", "White"),
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
