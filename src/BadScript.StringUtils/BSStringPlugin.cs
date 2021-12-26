using BadScript.Plugins;

namespace BadScript.StringUtils
{
    public class BSStringPlugin : Plugin<BSEngineSettings>
    {
        public BSStringPlugin() : base("BadScript.StringUtils", "String Utility API", "Tim Akermann", typeof(BSStringPlugin).Assembly.GetName().Version)
        {
        }
        public override void Load(BSEngineSettings settings)
        {
            settings.Interfaces.Add(new BSStringInterface());
        }
    }
}