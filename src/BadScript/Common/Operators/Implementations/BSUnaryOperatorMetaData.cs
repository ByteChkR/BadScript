using BadScript.Common.Runtime;
using BadScript.Common.Types;

namespace BadScript.Common.Operators.Implementations
{

    public class BSUnaryOperatorMetaData
    {
        public string OperatorKey;
        public string ImplementationOperatorKey;
        public string Signature;
        public int ArgumentCount;

        public BSUnaryOperatorMetaData(string op, string implKey, string sig, int argc)
        {
            OperatorKey = op;
            ImplementationOperatorKey = implKey;
            Signature = sig;
            ArgumentCount = argc;
        }

        public BSFunction MakeFunction()
        {
            return new BSFunction(
                $"function {OperatorKey}({Signature})",
                objects =>
                    BSOperatorImplementationResolver.
                        ResolveImplementation(ImplementationOperatorKey, objects).
                        ExecuteOperator(objects),
                ArgumentCount
            );
        }
    }

}