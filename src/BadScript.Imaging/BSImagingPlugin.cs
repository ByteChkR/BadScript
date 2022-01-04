using BadScript.Plugins;

namespace BadScript.Imaging
{
    public class BSImagingPlugin : Plugin<BSEngineSettings>
    {
        public BSImagingPlugin() : base("BadScript.Imaging", "Image/Drawing API", "Tim Akermann", typeof(BSImagingPlugin).Assembly.GetName().Version)
        {
        }
        public override void Load(BSEngineSettings settings)
        {
            settings.Interfaces.Add(new BSDrawingInterface());
        }
    }
}