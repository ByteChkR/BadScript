namespace BadScript.Parser.OperatorImplementations.Implementations.Math.Self
{

    public class BSSelfModuloOperatorImplementation : ABSSelfMathOperatorImplementation
    {

        #region Public

        public BSSelfModuloOperatorImplementation() : base( "%=" )
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
