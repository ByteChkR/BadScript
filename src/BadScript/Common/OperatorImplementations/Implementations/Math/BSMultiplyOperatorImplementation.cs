namespace BadScript.Common.OperatorImplementations.Implementations.Math
{

    public class BSMultiplyOperatorImplementation : ABSMathOperatorImplementation
    {

        #region Public

        public BSMultiplyOperatorImplementation() : base( "*" )
        {
        }

        #endregion

        #region Protected

        protected override decimal Execute( decimal l, decimal r )
        {
            return l * r;
        }

        #endregion

    }

}
