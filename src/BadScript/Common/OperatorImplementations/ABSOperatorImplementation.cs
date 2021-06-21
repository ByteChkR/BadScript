using BadScript.Common.Types;

namespace BadScript.Common.OperatorImplementations
{

    public abstract class ABSOperatorImplementation
    {

        public readonly string OperatorKey;

        #region Public

        public abstract bool IsCorrectImplementation( ABSObject[] args );

        public ABSObject ExecuteOperator( ABSObject[] arg )
        {
            return Execute( arg );
        }

        #endregion

        #region Protected

        protected ABSOperatorImplementation( string key )
        {
            OperatorKey = key;
        }

        protected abstract ABSObject Execute( ABSObject[] args );

        #endregion

    }

}
