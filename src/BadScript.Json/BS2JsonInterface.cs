using BadScript.Interfaces;
using BadScript.Types;
using BadScript.Types.Implementations;

namespace BadScript.Json
{
    public class BS2JsonInterface : ABSScriptInterface
    {

        #region Public

        public BS2JsonInterface() : base( "Json" )
        {
        }

        public override void AddApi( ABSTable root )
        {
            BSFunction f = new BSFunction( "function ToJson(jsonStr)", BS2Json.Convert, 1 );
            root.InsertElement( new BSObject( "ToJson" ), f );
        }

        #endregion

    }

}
