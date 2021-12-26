using BadScript.Plugins;

namespace BadScript.HttpServer
{
    public class BSHttpServerPlugin : Plugin<BSEngineSettings>
    {
        public BSHttpServerPlugin() : base("BadScript.HttpServer", "HttpServer Hosting API", "Tim Akermann", typeof(BSHttpServerPlugin).Assembly.GetName().Version)
        {
        }
        public override void Load(BSEngineSettings settings)
        {
            settings.Interfaces.Add(new BSHttpServerInterface());
        }
    }
}