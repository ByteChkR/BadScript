using BadScript.Plugins;

namespace BadScript.Json
{
    public class BSJsonPlugin : Plugin<BSEngineSettings>
    {
        public BSJsonPlugin() : base("BadScript.Json", "Json Serialization API", "Tim Akermann", typeof(BSJsonPlugin).Assembly.GetName().Version)
        {
        }
        public override void Load(BSEngineSettings settings)
        {
            settings.Interfaces.Add(new BS2JsonInterface());
            settings.Interfaces.Add(new Json2BSInterface());
        }
    }
}