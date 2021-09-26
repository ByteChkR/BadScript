﻿namespace BadScript.Common.OperatorImplementations.Implementations.Logic.Self
{

    public class BSSelfAndOperatorImplementation : ABSSelfLogicOperatorImplementation
    {

        #region Public

        public BSSelfAndOperatorImplementation() : base( "&=" )
        {
        }

        public override bool Execute( bool l, bool r )
        {
            return l && r;
        }

        #endregion

    }

}
