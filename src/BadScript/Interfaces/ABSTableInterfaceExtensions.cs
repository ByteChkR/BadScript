using BadScript.Types;
using BadScript.Types.Implementations;

namespace BadScript.Interfaces
{

    public static class ABSTableInterfaceExtensions
    {

        #region Public

        public static void InsertElement( this ABSTable t, string key, ABSObject value )
        {
            t.InsertElement( new BSObject( key ), value );
        }

        public static void InsertElement( this ABSTable t, string key, string value )
        {
            t.InsertElement( new BSObject( key ), new BSObject( value ) );
        }

        #endregion

    }

}
