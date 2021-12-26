using BadScript.Plugins;

namespace BadScript.Math
{
    public class BSProcessPlugin : Plugin<BSEngineSettings>
    {
        public BSProcessPlugin() : base("BadScript.Math", "Math API", "Tim Akermann", typeof(BSProcessPlugin).Assembly.GetName().Version)
        {
        }
        public override void Load(BSEngineSettings settings)
        {
            settings.Interfaces.Add(new BSMathInterface());
        }
    }
}