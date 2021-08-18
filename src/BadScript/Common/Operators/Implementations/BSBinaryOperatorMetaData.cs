using BadScript.Common.Runtime;
using BadScript.Common.Types;

namespace BadScript.Common.Operators.Implementations
{

    public class BSBinaryOperatorMetaData
    {
        public string OperatorKey;
        public string Signature;
        public int ArgumentCount;

        public BSBinaryOperatorMetaData( string op, string sig, int argc )
        {
            OperatorKey = op;
            Signature = sig;
            ArgumentCount = argc;
        }

        public BSFunction MakeFunction()
        {
            return new BSFunction(
                $"function {OperatorKey}({Signature})",
                objects => BSOperatorImplementationResolver.
                           ResolveImplementation(OperatorKey, objects).
                           ExecuteOperator(objects),
                ArgumentCount
            );
        }
    }

}