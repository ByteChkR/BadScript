using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Utils.Optimization.Compilation;

namespace BadScript.Utils.Optimization
{

    public class BSExpressionOptimizerMetaData
    {
        public BSCompiledExpressionCode ExpressionCode;
        public object Value;

        #region Public

        public BSExpressionOptimizerMetaData( object o )
        {
            Value = o;

            if ( o == null )
            {
                ExpressionCode = BSCompiledExpressionCode.ValueNull;
            }
            else if ( o is string )
            {
                ExpressionCode = BSCompiledExpressionCode.ValueString;
            }
            else if ( o is decimal )
            {
                ExpressionCode = BSCompiledExpressionCode.ValueDecimal;
            }
            else
            {
                throw new BSInvalidOperationException(
                    SourcePosition.Unknown,
                    "Can not Create Meta Data for object: " + o );
            }
        }

        #endregion
    }

}
