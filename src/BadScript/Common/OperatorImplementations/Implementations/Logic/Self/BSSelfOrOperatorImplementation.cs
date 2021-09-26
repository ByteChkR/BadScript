namespace BadScript.Common.OperatorImplementations.Implementations.Logic.Self
{

    public class BSSelfOrOperatorImplementation : ABSSelfLogicOperatorImplementation
    {

        #region Public

        public BSSelfOrOperatorImplementation() : base( "|=" )
        {
        }

        public override bool Execute( bool l, bool r )
        {
            return l || r;
        }

        #endregion

    }

}
