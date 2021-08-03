namespace BadScript.Common.OperatorImplementations.Implementations
{

    public class BSGreaterOrEqualOperatorImplementation : ABSRelationalOperatorImplementation
    {
        #region Public

        public BSGreaterOrEqualOperatorImplementation() : base( ">=" )
        {
        }

        #endregion

        #region Protected

        protected override bool Execute( decimal l, decimal r )
        {
            return l >= r;
        }

        #endregion
    }

}
