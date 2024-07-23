using HarmonyLib;
using System.Reflection;
using System.Xml.Linq;
using System.IO;


namespace CustomDewCollectorSize
{
    public class ModLoader : IModApi
    {
        public static string Version = "v1.0";
        public static bool IsEnabled { get; set; }
        public static int Columns { get; set; }
        public static int Rows { get; set; }

        public void InitMod(Mod _modInstance)
        {
            LoadConfig();
            var harmony = new Harmony(_modInstance.Name);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Log.Out($"CustomDewCollectorSize {Version} Loaded");
            Log.Out($"CustomDewCollectorSize mod is {(IsEnabled ? "enabled" : "disabled")}.");
        }

        public static void LoadConfig()
        {
            var configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config.xml");
            if (File.Exists(configPath))
            {
                var configXml = XDocument.Load(configPath);
                // load enabled
                var enabledElement = configXml.Root.Element("enabled");
                if (enabledElement != null && bool.TryParse(enabledElement.Value, out bool isEnabled))
                {
                    IsEnabled = isEnabled;
                }
                // Load columns
                var columnsElement = configXml.Root.Element("columns");
                if (columnsElement != null && int.TryParse(columnsElement.Value, out int columns))
                {
                    Columns = columns;
                }
                // Load rows
                var rowsElement = configXml.Root.Element("rows");
                if (rowsElement != null && int.TryParse(rowsElement.Value, out int rows))
                {
                    Rows = rows;
                }
            }
        }
    }
}
