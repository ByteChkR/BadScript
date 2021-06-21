namespace BadScript.Common.OperatorImplementations.Implementations
{

    public class BSXOrDefaultOperatorImplementation : ABSLogicOperatorImplementation
    {

        #region Public

        public BSXOrDefaultOperatorImplementation() : base( "^" )
        {
        }

        public override bool Execute( bool l, bool r )
        {
            return l ^ r;
        }

        #endregion

    }

}
