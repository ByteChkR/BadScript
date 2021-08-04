namespace BadScript.Common.OperatorImplementations.Implementations.Logic
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
