namespace BadScript.Common.OperatorImplementations.Implementations
{

    public class BSGreaterThanOperatorImplementation : BSRelationalOperatorImplementation
    {
        #region Public

        public BSGreaterThanOperatorImplementation() : base( ">" )
        {
        }

        #endregion

        #region Protected

        protected override bool Execute( decimal l, decimal r )
        {
            return l > r;
        }

        #endregion
    }

}
