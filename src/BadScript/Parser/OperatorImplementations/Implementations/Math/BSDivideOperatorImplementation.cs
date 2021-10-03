namespace BadScript.Parser.OperatorImplementations.Implementations.Math
{

    public class BSDivideOperatorImplementation : ABSMathOperatorImplementation
    {

        #region Public

        public BSDivideOperatorImplementation() : base( "/" )
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
