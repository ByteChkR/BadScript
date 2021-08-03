namespace BadScript.Common.OperatorImplementations.Implementations
{

    public class BSMinusOperatorImplementation : ABSMathOperatorImplementation
    {
        #region Public

        public BSMinusOperatorImplementation() : base( "-" )
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
