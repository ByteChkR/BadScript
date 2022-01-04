using BadScript.Plugins;

namespace BadScript.Xml
{
    public class BSXmlPlugin : Plugin<BSEngineSettings>
    {
        public BSXmlPlugin() : base("BadScript.Xml", "Xml Serializer API", "Tim Akermann", typeof(BSXmlPlugin).Assembly.GetName().Version)
        {
        }
        public override void Load(BSEngineSettings settings)
        {
            settings.Interfaces.Add(new BSXmlInterface());
        }
    }
}