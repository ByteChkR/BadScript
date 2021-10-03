using BadScript.Parser.OperatorImplementations;
using BadScript.Types;

namespace BadScript.Parser.Operators.Implementations
{

    public class BSUnaryOperatorMetaData
    {

        public string OperatorKey;
        public string ImplementationOperatorKey;
        public string Signature;
        public int ArgumentCount;

        #region Public

        public BSUnaryOperatorMetaData( string op, string implKey, string sig, int argc )
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
                                          ResolveImplementation( ImplementationOperatorKey, objects ).
                                          ExecuteOperator( objects ),
                                  ArgumentCount
                                 );
        }

        #endregion

    }

}
