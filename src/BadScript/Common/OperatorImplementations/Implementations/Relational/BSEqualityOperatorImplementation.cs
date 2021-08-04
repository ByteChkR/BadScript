﻿using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.OperatorImplementations.Implementations.Relational
{

    public class BSEqualityOperatorImplementation : ABSOperatorImplementation
    {
        #region Public

        public BSEqualityOperatorImplementation() : base( "==" )
        {
        }

        public override bool IsCorrectImplementation( ABSObject[] args )
        {
            return true;
        }

        #endregion

        #region Protected

        protected override ABSObject Execute( ABSObject[] arg )
        {
            return arg[0].Equals( arg[1] ) ? BSObject.One : BSObject.Zero;
        }

        #endregion
    }

}
