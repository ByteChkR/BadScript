using BadScript.Interfaces;
using BadScript.Types;
using BadScript.Types.Implementations;

namespace BadScript.Json
{

    public class Json2BSInterface : ABSScriptInterface
    {

        #region Public

        public Json2BSInterface() : base( "Json" )
        {
        }

        public override void AddApi( ABSTable root )
        {
            BSFunction f = new BSFunction(
                                          "function FromJson(jsonStr)/FromJson(jsonStr, baseObj)",
                                          Json2BS.Convert,
                                          1,
                                          2
                                         );

            root.InsertElement( new BSObject( "FromJson" ), f );
        }

        #endregion

    }

}
