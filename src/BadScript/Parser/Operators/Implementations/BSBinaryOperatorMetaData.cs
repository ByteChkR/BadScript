using BadScript.Parser.OperatorImplementations;
using BadScript.Types;

namespace BadScript.Parser.Operators.Implementations
{

    public class BSBinaryOperatorMetaData
    {

        public string OperatorKey;
        public string Signature;
        public int ArgumentCount;

        #region Public

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
                                             ResolveImplementation( OperatorKey, objects ).
                                             ExecuteOperator( objects ),
                                  ArgumentCount
                                 );
        }

        #endregion

    }

}
