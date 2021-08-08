using BadScript.Common.Exceptions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Tools.CodeGenerator.Runtime
{

    public static class WrapperHelper
    {
        public static T UnwrapObject<T>(ABSObject o)
        {
            //TODO Convert Numbers and Strings
            if(o is BSWrapperObject <T> ob )
            {
                return ob.GetInternalObject();
            }
            else if ( o is BSObject obj )
                return (T)obj.GetInternalObject();
            throw new BSRuntimeException( "Can not Unwrap Object" );
        }
    }

}
