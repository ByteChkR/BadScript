using BadScript.Plugins;
using BadScript.Types.Implementations;

namespace BadScript.StringUtils
{

    public class BSStringPlugin : Plugin < BSEngineSettings >
    {

        #region Public

        public BSStringPlugin() : base(
                                       "BadScript.StringUtils",
                                       "String Utility API",
                                       "Tim Akermann",
                                       typeof( BSStringPlugin ).Assembly.GetName().Version
                                      )
        {
        }

        public override void Load( BSEngineSettings settings )
        {
            BSStringInterface api = new BSStringInterface();
            settings.Interfaces.Add( api );

            BSObjectExtension ext = new BSObjectExtension( api.Api );
            settings.AddExtension < string >( ext );
        }

        #endregion

    }

}
