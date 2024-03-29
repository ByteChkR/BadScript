﻿namespace BadScript.Parser.OperatorImplementations.Implementations.Logic
{

    public class BSXOrOperatorImplementation : ABSLogicOperatorImplementation
    {

        #region Public

        public BSXOrOperatorImplementation() : base( "^" )
        {
        }

        public override bool Execute( bool l, bool r )
        {
            return l ^ r;
        }

        #endregion

    }

}
