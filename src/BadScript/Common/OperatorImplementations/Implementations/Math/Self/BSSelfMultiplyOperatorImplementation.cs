﻿namespace BadScript.Common.OperatorImplementations.Implementations
{

    public class BSSelfMultiplyOperatorImplementation : ABSSelfMathOperatorImplementation
    {
        #region Public

        public BSSelfMultiplyOperatorImplementation() : base( "*=" )
        {
        }

        #endregion

        #region Protected

        protected override decimal Execute( decimal l, decimal r )
        {
            return l * r;
        }

        #endregion
    }

}
