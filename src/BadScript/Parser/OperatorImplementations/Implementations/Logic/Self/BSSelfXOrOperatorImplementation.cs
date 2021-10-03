namespace BadScript.Parser.OperatorImplementations.Implementations.Logic.Self
{

    public class BSSelfXOrOperatorImplementation : ABSSelfLogicOperatorImplementation
    {

        #region Public

        public BSSelfXOrOperatorImplementation() : base( "^=" )
        {
        }

        public override bool Execute( bool l, bool r )
        {
            return l ^ r;
        }

        #endregion

    }

}
