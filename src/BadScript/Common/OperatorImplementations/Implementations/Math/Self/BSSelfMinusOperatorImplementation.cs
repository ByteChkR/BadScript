namespace BadScript.Common.OperatorImplementations.Implementations
{

    public class BSSelfMinusOperatorImplementation : ABSSelfMathOperatorImplementation
    {
        #region Public

        public BSSelfMinusOperatorImplementation() : base( "-=" )
        {
        }

        #endregion

        #region Protected

        protected override decimal Execute( decimal l, decimal r )
        {
            return l - r;
        }

        #endregion
    }

}
