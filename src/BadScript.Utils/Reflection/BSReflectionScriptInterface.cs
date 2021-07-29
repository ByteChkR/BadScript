using System;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Utils
{

    public class BSReflectionScriptInterface: ABSScriptInterface
    {
        public BSReflectionScriptInterface() : base( "reflection" )
        {
        }

        public override void AddApi( ABSTable root )
        {
            root.InsertElement(new BSObject("getType"), new BSFunction("function getType(fullName)", GetType, 1));
            root.InsertElement(new BSObject("wrapInstance"), new BSFunction("function wrapInstance(instance)", WrapInstance, 1));
            root.InsertElement(new BSObject("getConstructorData"), new BSFunction("function getConstructorData()", objects => TypeBuilder.GetConstructorData(), 0));
        }

        private ABSObject WrapInstance(ABSObject[] arg)
        {
            object o = ( ( BSObject ) arg[0] ).GetInternalObject();

            return TypeBuilder.Build( o.GetType(), o );
        }

        private ABSObject GetType( ABSObject[] arg )
        {
            string s = arg[0].ConvertString();

            return TypeBuilder.Build( Type.GetType( s ) );
        }
    }

}
