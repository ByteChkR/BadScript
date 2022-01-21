using BadScript.Plugins;

namespace BadScript.Process
{

    public class BSProcessPlugin : Plugin < BSEngineSettings >
    {

        #region Public

        public BSProcessPlugin() : base(
                                        "BadScript.Process",
                                        "OS Process API",
                                        "Tim Akermann",
                                        typeof( BSProcessPlugin ).Assembly.GetName().Version
                                       )
        {
        }

        public override void Load( BSEngineSettings settings )
        {
            settings.Interfaces.Add( new BSProcessInterface() );
        }

        #endregion

    }

}
