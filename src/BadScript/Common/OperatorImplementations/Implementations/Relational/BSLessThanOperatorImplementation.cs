﻿namespace BadScript.Common.OperatorImplementations.Implementations
{

    public class BSLessThanOperatorImplementation : ABSRelationalOperatorImplementation
    {
        #region Public

        public BSLessThanOperatorImplementation() : base( "<" )
        {
        }

        #endregion

        #region Protected

        protected override bool Execute( decimal l, decimal r )
        {
            return l < r;
        }

        #endregion
    }

}