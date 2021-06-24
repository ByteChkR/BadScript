namespace BadScript.Common.OperatorImplementations.Implementations
{

    public class BSOrOperatorImplementation : ABSLogicOperatorImplementation
    {
        #region Public

        public BSOrOperatorImplementation() : base( "||" )
        {
        }

        public override bool Execute( bool l, bool r )
        {
            return l || r;
        }

        #endregion
    }

}
