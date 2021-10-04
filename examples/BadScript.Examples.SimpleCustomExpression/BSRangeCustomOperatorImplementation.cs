using BadScript.Parser.OperatorImplementations;
using BadScript.Types;

namespace BadScript.Examples.SimpleCustomExpression
{

    public class BSRangeCustomOperatorImplementation: ABSOperatorImplementation
    {

        public BSRangeCustomOperatorImplementation(  ) : base( ".." )
        {
        }

        public override bool IsCorrectImplementation( ABSObject[] args )
        {
            return args.Length == 2 &&
                   args[0].TryConvertDecimal( out decimal _ ) &&
                   args[1].TryConvertDecimal( out decimal _ );
        }

        protected override ABSObject Execute( ABSObject[] args )
        {
            decimal dStart = args[0].ConvertDecimal();
            decimal dEnd = args[1].ConvertDecimal();

            return new BSRangeEnumerableObject(args[0].Position, (int)dStart, (int)dEnd);
        }

    }

}