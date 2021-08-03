namespace BadScript.Common.OperatorImplementations.Implementations
{

    public class BSGreaterThanOperatorImplementation : ABSRelationalOperatorImplementation
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
