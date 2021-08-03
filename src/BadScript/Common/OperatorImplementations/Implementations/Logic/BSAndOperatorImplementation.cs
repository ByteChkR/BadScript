namespace BadScript.Common.OperatorImplementations.Implementations
{

    public class BSAndOperatorImplementation : ABSLogicOperatorImplementation
    {
        #region Public

        public BSAndOperatorImplementation() : base( "&&" )
        {
        }

        public override bool Execute( bool l, bool r )
        {
            return l && r;
        }

        #endregion
    }

}
