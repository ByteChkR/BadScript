using BadScript.Exceptions;
using BadScript.Parser.Expressions;

namespace BadScript.Validators
{

    public abstract class BSExpressionValidator
    {

        public class BSValidatorException : BSRuntimeException
        {

            public readonly BSExpression[] Expressions;

            #region Public

            public BSValidatorException( string message, BSExpression[] exprs ) : base( message )
            {
                Expressions = exprs;
            }

            #endregion

        }

        public abstract string ValidatorName { get; }

        #region Public

        public abstract void Validate( BSExpression[] expressions );

        #endregion

    }

}
