using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Types;
using BadScript.Common.Types.References;

namespace BadScript.Common.OperatorImplementations.Implementations
{

    public class BSArrayAccessOperatorImplementation:ABSOperatorImplementation
    {
        public BSArrayAccessOperatorImplementation() : base( "[]" )
        {

        }

        public override bool IsCorrectImplementation( ABSObject[] args )
        {
            return true;
        }

        protected override ABSObject Execute( ABSObject[] args )
        {
            ABSObject obj = args[0].ResolveReference();
            ABSObject i = args[1];
            if (obj is ABSTable t)
            {
                return t.GetElement(i);
            }

            if (obj is ABSArray a)
            {
                if (i.TryConvertDecimal(out decimal d))
                {
                    return a.GetElement((int)d);
                }
            }

            throw new BSInvalidTypeException(
                SourcePosition.Unknown, 
                "Expected Array",
                obj,
                "Table"
            );
        }
    }

}