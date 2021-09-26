namespace BadScript.Common.OperatorImplementations.Implementations.Math
{

    public class BSModuloOperatorImplementation : ABSMathOperatorImplementation
    {

        #region Public

        public BSModuloOperatorImplementation() : base( "%" )
        {
        }

        #endregion

        #region Protected

        protected override decimal Execute( decimal l, decimal r )
        {
            return l % r;
        }

        #endregion

    }

}
