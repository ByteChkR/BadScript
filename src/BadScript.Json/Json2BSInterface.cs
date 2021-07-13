using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Json
{

    public class Json2BSInterface : ABSScriptInterface
    {
        #region Public

        public Json2BSInterface() : base( "json" )
        {
        }

        public override void AddApi( ABSTable root )
        {
            BSFunction f = new BSFunction( "function fromJson(jsonStr)", Json2BS.Convert, 1 );
            root.InsertElement( new BSObject( "fromJson" ), f );

        }

        #endregion
    }

}
