using BadScript.Plugins;

namespace BadScript.Zip
{
    public class BSZipPlugin : Plugin<BSEngineSettings>
    {
        public BSZipPlugin() : base("BadScript.Zip", "Zip Serializer API", "Tim Akermann", typeof(BSZipPlugin).Assembly.GetName().Version)
        {
        }
        public override void Load(BSEngineSettings settings)
        {
            settings.Interfaces.Add(new BSZipInterface());
        }
    }
}