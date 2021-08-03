﻿namespace BadScript.Common.OperatorImplementations.Implementations
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