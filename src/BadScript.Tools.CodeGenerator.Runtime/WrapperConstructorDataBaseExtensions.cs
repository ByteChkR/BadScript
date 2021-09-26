using BadScript.Common.Types;

namespace BadScript.Tools.CodeGenerator.Runtime
{

    public static class WrapperConstructorDataBaseExtensions
    {

        #region Public

        public static ABSObject Get < T >( this IWrapperConstructorDataBase db, object[] args )
        {
            return db.Get( typeof( T ), args );
        }

        public static bool HasType < T >( this IWrapperConstructorDataBase db )
        {
            return db.HasType( typeof( T ) );
        }

        #endregion

    }

}
