namespace BadScript.Common.OperatorImplementations.Implementations
{

    public class BSLessOrEqualOperatorImplementation : ABSRelationalOperatorImplementation
    {
        #region Public

        public BSLessOrEqualOperatorImplementation() : base( "<=" )
        {
        }

        #endregion

        #region Protected

        protected override bool Execute( decimal l, decimal r )
        {
            return l <= r;
        }

        #endregion
    }

}
