using System;
using BadScript.Plugins;

namespace BadScript.IO
{
    public class BSFileSystemPlugin : Plugin<BSEngineSettings>
    {
        public BSFileSystemPlugin() : base("BadScript.IO", "OS FileSystem API", "Tim Akermann", typeof(BSFileSystemPlugin).Assembly.GetName().Version)
        {
        }
        public override void Load(BSEngineSettings settings)
        {
            settings.Interfaces.Add(new BSFileSystemInterface());
            settings.Interfaces.Add(new BSFileSystemPathInterface(AppDomain.CurrentDomain.BaseDirectory));
        }
    }
}