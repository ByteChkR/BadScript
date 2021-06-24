﻿using System;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.OperatorImplementations.Implementations
{

    public class BSAddOperatorImplementation : ABSOperatorImplementation
    {
        #region Public

        public BSAddOperatorImplementation() : base( "+" )
        {
        }

        public static ABSObject Add( ABSObject[] arg )
        {
            ABSObject lVal = arg[0];
            ABSObject rVal = arg[1];

            if ( lVal.TryConvertDecimal( out decimal lD ) )
            {
                if ( rVal.TryConvertDecimal( out decimal rD ) )
                {
                    return new BSObject( lD + rD );
                }

                if ( rVal.TryConvertString( out string rS ) )
                {
                    return new BSObject( lD + rS );
                }

                throw new Exception( $"'{rVal}' can not be added to '{lVal}'" );
            }

            if ( lVal.TryConvertString( out string lS ) &&
                 rVal.TryConvertString( out string rStr ) )
            {
                return new BSObject( lS + rStr );
            }

            throw new Exception( $"'{rVal}' can not be added" );
        }

        public override bool IsCorrectImplementation( ABSObject[] arg )
        {
            ABSObject lVal = arg[0];
            ABSObject rVal = arg[1];

            return lVal.TryConvertDecimal( out decimal _ ) &&
                   ( rVal.TryConvertDecimal( out decimal _ ) || rVal.TryConvertString( out string _ ) ) ||
                   lVal.TryConvertString( out string _ ) && rVal.TryConvertString( out string _ );
        }

        #endregion

        #region Protected

        protected override ABSObject Execute( ABSObject[] arg )
        {
            return Add( arg );
        }

        #endregion
    }

    public class BSNullTestOperatorImplementation : ABSOperatorImplementation
    {
        #region Public

        public BSNullTestOperatorImplementation() : base( "??" )
        {
        }

        public static ABSObject NullTest( ABSObject[] arg )
        {
            ABSObject lVal = arg[0];
            ABSObject rVal = arg[1];

            return lVal.IsNull ? rVal : lVal;
        }

        public override bool IsCorrectImplementation( ABSObject[] arg )
        {
            return true;
        }

        #endregion

        #region Protected

        protected override ABSObject Execute( ABSObject[] arg )
        {
            return NullTest( arg );
        }

        #endregion
    }

}
