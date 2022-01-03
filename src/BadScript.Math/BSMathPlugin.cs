using BadScript.Plugins;

namespace BadScript.Math
{
    public class BSMathPlugin : Plugin<BSEngineSettings>
    {
        public BSMathPlugin() : base("BadScript.Math", "Math API", "Tim Akermann", typeof(BSMathPlugin).Assembly.GetName().Version)
        {
        }
        public override void Load(BSEngineSettings settings)
        {
            settings.Interfaces.Add(new BSMathInterface());
        }
    }
}