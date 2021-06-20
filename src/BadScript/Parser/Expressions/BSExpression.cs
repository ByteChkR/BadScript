using BadScript.Runtime;

namespace BadScript.Parser.Expressions
{

    public abstract class BSExpression
    {

        #region Public
        public abstract BSRuntimeObject Execute( BSEngineScope scope );
        
        #endregion

    }

}
