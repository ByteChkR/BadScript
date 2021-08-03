﻿using BadScript.Common.Exceptions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Common.OperatorImplementations.Implementations
{

    public class BSPrefixIncrementOperatorImplementation : ABSOperatorImplementation
    {
        #region Public

        public BSPrefixIncrementOperatorImplementation() : base( "++_Pre" )
        {
        }

        public override bool IsCorrectImplementation( ABSObject[] args )
        {
            return args[0].TryConvertDecimal( out decimal _ );
        }

        #endregion

        #region Protected

        protected override ABSObject Execute( ABSObject[] args )
        {
            ABSObject left = args[0];

            if ( left is ABSReference lRef )
            {
                lRef.Assign( new BSObject( lRef.ConvertDecimal() + 1 ) );

                return lRef;
            }
            else
            {
                throw new BSInvalidTypeException(
                    left.Position,
                    "Expected Assignable Reference",
                    left,
                    "Reference"
                );
            }

        }

        #endregion
    }

    public class BSPostfixIncrementOperatorImplementation : ABSOperatorImplementation
    {
        #region Public

        public BSPostfixIncrementOperatorImplementation() : base( "++_Post" )
        {
        }

        public override bool IsCorrectImplementation( ABSObject[] args )
        {
            return args[0].TryConvertDecimal( out decimal _ );
        }

        #endregion

        #region Protected

        protected override ABSObject Execute( ABSObject[] args )
        {
            ABSObject left = args[0];

            if ( left is ABSReference lRef )
            {
                ABSObject oldVal = lRef.ResolveReference();
                lRef.Assign( new BSObject( lRef.ConvertDecimal() + 1 ) );

                return oldVal;
            }
            else
            {
                throw new BSInvalidTypeException(
                    left.Position,
                    "Expected Assignable Reference",
                    left,
                    "Reference"
                );
            }

        }

        #endregion
    }

    public class BSPrefixDecrementOperatorImplementation : ABSOperatorImplementation
    {
        #region Public

        public BSPrefixDecrementOperatorImplementation() : base( "--_Pre" )
        {
        }

        public override bool IsCorrectImplementation( ABSObject[] args )
        {
            return args[0].TryConvertDecimal( out decimal _ );
        }

        #endregion

        #region Protected

        protected override ABSObject Execute( ABSObject[] args )
        {
            ABSObject left = args[0];

            if ( left is ABSReference lRef )
            {
                lRef.Assign( new BSObject( lRef.ConvertDecimal() - 1 ) );

                return lRef;
            }
            else
            {
                throw new BSInvalidTypeException(
                    left.Position,
                    "Expected Assignable Reference",
                    left,
                    "Reference"
                );
            }

        }

        #endregion
    }

    public class BSPostfixDecrementOperatorImplementation : ABSOperatorImplementation
    {
        #region Public

        public BSPostfixDecrementOperatorImplementation() : base( "--_Post" )
        {
        }

        public override bool IsCorrectImplementation( ABSObject[] args )
        {
            return args[0].TryConvertDecimal( out decimal _ );
        }

        #endregion

        #region Protected

        protected override ABSObject Execute( ABSObject[] args )
        {
            ABSObject left = args[0];

            if ( left is ABSReference lRef )
            {
                ABSObject oldVal = lRef.ResolveReference();
                lRef.Assign( new BSObject( lRef.ConvertDecimal() - 1 ) );

                return oldVal;
            }
            else
            {
                throw new BSInvalidTypeException(
                    left.Position,
                    "Expected Assignable Reference",
                    left,
                    "Reference"
                );
            }

        }

        #endregion
    }

    public class BSSelfAddOperatorImplementation : ABSOperatorImplementation
    {
        #region Public

        public BSSelfAddOperatorImplementation() : base( "+=" )
        {
        }

        public static ABSObject Add( ABSObject[] arg )
        {
            ABSObject left = arg[0];
            ABSObject right = arg[1];

            if ( left is ABSReference lRef )
            {
                ABSObject val = null;

                if ( left.TryConvertDecimal( out decimal lD ) )
                {
                    if ( right.TryConvertDecimal( out decimal rD ) )
                    {
                        val = new BSObject( lD + rD );
                    }
                    else if ( right.TryConvertString( out string rS ) )
                    {
                        val = new BSObject( lD + rS );
                    }
                }
                else if ( left.TryConvertString( out string lS ) &&
                          right.TryConvertString( out string rStr ) )
                {
                    val = new BSObject( lS + rStr );
                }

                if ( val == null )
                {
                    throw new BSInvalidOperationException( left.Position, "+=(AddSelf)", left, right );
                }

                lRef.Assign( val );

                return lRef;
            }
            else
            {
                throw new BSInvalidTypeException(
                    left.Position,
                    "Expected Assignable Reference",
                    left,
                    "Reference"
                );
            }

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

}