using BadScript.Plugins;

namespace BadScript.Threading
{
    public class BSThreadingPlugin : Plugin<BSEngineSettings>
    {
        public BSThreadingPlugin() : base("BadScript.Threading", "Multithreading API", "Tim Akermann", typeof(BSThreadingPlugin).Assembly.GetName().Version)
        {
        }
        public override void Load(BSEngineSettings settings)
        {
            settings.Interfaces.Add(new BSThreadingInterface());
        }
    }
}