namespace BadScript.Common.OperatorImplementations.Implementations
{

    public class BSSelfDivideOperatorImplementation : ABSSelfMathOperatorImplementation
    {
        #region Public

        public BSSelfDivideOperatorImplementation() : base( "/=" )
        {
        }

        #endregion

        #region Protected

        protected override decimal Execute( decimal l, decimal r )
        {
            return l / r;
        }

        #endregion
    }

}
