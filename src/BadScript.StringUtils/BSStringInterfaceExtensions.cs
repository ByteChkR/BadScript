using BadScript.Types;
using BadScript.Types.Implementations;

namespace BadScript.StringUtils
{

    public static class BSStringInterfaceExtensions
    {

        #region Public

        public static void InsertElement( this ABSTable t, string name, ABSObject o )
        {
            t.InsertElement( new BSObject( name ), o );
        }

        #endregion

    }

}
