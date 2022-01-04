using BadScript.Plugins;

namespace BadScript.ConsoleUtils
{
    public class BSConsolePlugin : Plugin<BSEngineSettings>
    {
        public BSConsolePlugin() : base("BadScript.ConsoleUtils", "System Console IO API", "Tim Akermann", typeof(BSConsolePlugin).Assembly.GetName().Version)
        {
        }
        public override void Load(BSEngineSettings settings)
        {
            settings.Interfaces.Add(new BSSystemConsoleInterface());
        }
    }
}