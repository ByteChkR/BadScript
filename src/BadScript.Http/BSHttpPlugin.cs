using BadScript.Plugins;

namespace BadScript.Http
{

    public class BSHttpPlugin : Plugin < BSEngineSettings >
    {

        #region Public

        public BSHttpPlugin() : base(
                                     "BadScript.Http",
                                     "Http API",
                                     "Tim Akermann",
                                     typeof( BSHttpPlugin ).Assembly.GetName().Version
                                    )
        {
        }

        public override void Load( BSEngineSettings settings )
        {
            settings.Interfaces.Add( new BSHttpInterface() );
        }

        #endregion

    }

}
