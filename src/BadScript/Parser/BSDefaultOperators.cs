using System;

using BadScript.Runtime;
using BadScript.Runtime.Implementations;

namespace BadScript.Parser
{

    public static class BSDefaultOperators
    {

        #region Public

        public static BSRuntimeObject OperatorAdd( BSRuntimeObject[] arg )
        {
            for ( int i = 0; i < arg.Length; i++ )
            {
                if ( arg[i] is BSRuntimeReference r )
                {
                    arg[i] = r.Get();
                }
            }

            BSRuntimeObject lVal = arg[0];
            BSRuntimeObject rVal = arg[1];

            if ( lVal.TryConvertDecimal( out decimal lD ) )
            {
                if ( rVal.TryConvertDecimal( out decimal rD ) )
                {
                    return new EngineRuntimeObject( lD + rD );
                }

                if ( rVal.TryConvertString( out string rS ) )
                {
                    return new EngineRuntimeObject( lD + rS );
                }

                throw new Exception( "Invalid Operator Usage" );
            }

            if ( lVal.TryConvertString( out string lS ) &&
                 rVal.TryConvertString( out string rStr ) )
            {
                return new EngineRuntimeObject( lS + rStr );
            }

            throw new Exception( "Invalid Operator Usage" );
        }

        public static BSRuntimeObject OperatorAnd( BSRuntimeObject[] arg )
        {
            BSRuntimeObject lVal = arg[0];
            BSRuntimeObject rVal = arg[1];

            if ( lVal.TryConvertBool( out bool lD ) )
            {
                if ( rVal.TryConvertBool( out bool rD ) )
                {
                    return new EngineRuntimeObject( lD && rD);
                }

                throw new Exception( "Invalid Operator Usage" );
            }

            throw new Exception( "Invalid Operator Usage" );
        }

        public static BSRuntimeObject OperatorDivide( BSRuntimeObject[] arg )
        {
            BSRuntimeObject lVal = arg[0];
            BSRuntimeObject rVal = arg[1];

            if ( lVal.TryConvertDecimal( out decimal lD ) )
            {
                if ( rVal.TryConvertDecimal( out decimal rD ) )
                {
                    return new EngineRuntimeObject( lD / rD );
                }

                throw new Exception( "Invalid Operator Usage" );
            }

            throw new Exception( "Invalid Operator Usage" );
        }

        public static BSRuntimeObject OperatorEquality(BSRuntimeObject[] arg)
        {
            return new EngineRuntimeObject((decimal)(arg[0].Equals(arg[1]) ? 1 : 0));
        }

        public static BSRuntimeObject OperatorInEquality(BSRuntimeObject[] arg)
        {
            return new EngineRuntimeObject((decimal)(arg[0].Equals(arg[1]) ? 0 : 1));
        }


        public static BSRuntimeObject OperatorLessThan(BSRuntimeObject[] arg)
        {
            if (arg[0].TryConvertDecimal(out decimal lD) &&
                arg[1].TryConvertDecimal(out decimal rD))
            {
                return new EngineRuntimeObject((decimal)(lD < rD ? 1:0));
            }
            throw new Exception("Invalid Operator Usage");
        }

        public static BSRuntimeObject OperatorGreaterThan(BSRuntimeObject[] arg)
        {
            if (arg[0].TryConvertDecimal(out decimal lD) &&
                arg[1].TryConvertDecimal(out decimal rD))
            {
                return new EngineRuntimeObject((decimal)(lD > rD ? 1 : 0));
            }
            throw new Exception("Invalid Operator Usage");
        }

        public static BSRuntimeObject OperatorLessOrEqual(BSRuntimeObject[] arg)
        {
            if (arg[0].TryConvertDecimal(out decimal lD) &&
                arg[1].TryConvertDecimal(out decimal rD))
            {
                return new EngineRuntimeObject((decimal)(lD <= rD ? 1 : 0));
            }
            throw new Exception("Invalid Operator Usage");
        }

        public static BSRuntimeObject OperatorGreaterOrEqual(BSRuntimeObject[] arg)
        {
            if (arg[0].TryConvertDecimal(out decimal lD) &&
                arg[1].TryConvertDecimal(out decimal rD))
            {
                return new EngineRuntimeObject((decimal)(lD >= rD ? 1 : 0));
            }
            throw new Exception("Invalid Operator Usage");
        }

        public static BSRuntimeObject OperatorMinus( BSRuntimeObject[] arg )
        {
            BSRuntimeObject lVal = arg[0];
            BSRuntimeObject rVal = arg[1];

            if ( lVal.TryConvertDecimal( out decimal lD ) )
            {
                if ( rVal.TryConvertDecimal( out decimal rD ) )
                {
                    return new EngineRuntimeObject( lD - rD );
                }

                throw new Exception( "Invalid Operator Usage" );
            }

            throw new Exception( "Invalid Operator Usage" );
        }

        public static BSRuntimeObject OperatorMod( BSRuntimeObject[] arg )
        {
            BSRuntimeObject lVal = arg[0];
            BSRuntimeObject rVal = arg[1];

            if ( lVal.TryConvertDecimal( out decimal lD ) )
            {
                if ( rVal.TryConvertDecimal( out decimal rD ) )
                {
                    return new EngineRuntimeObject( lD % rD );
                }

                throw new Exception( "Invalid Operator Usage" );
            }

            throw new Exception( "Invalid Operator Usage" );
        }

        public static BSRuntimeObject OperatorMultiply( BSRuntimeObject[] arg )
        {
            BSRuntimeObject lVal = arg[0];
            BSRuntimeObject rVal = arg[1];

            if ( lVal.TryConvertDecimal( out decimal lD ) )
            {
                if ( rVal.TryConvertDecimal( out decimal rD ) )
                {
                    return new EngineRuntimeObject( lD * rD );
                }

                throw new Exception( "Invalid Operator Usage" );
            }

            throw new Exception( "Invalid Operator Usage" );
        }

        public static BSRuntimeObject OperatorNot( BSRuntimeObject[] arg )
        {
            BSRuntimeObject lVal = arg[0];

            if ( lVal.TryConvertBool( out bool lD ) )
            {
                return new EngineRuntimeObject( ( decimal ) ( lD ? 1 : 0 ) );
            }

            throw new Exception( "Invalid Operator Usage" );
        }

        public static BSRuntimeObject OperatorOr( BSRuntimeObject[] arg )
        {
            BSRuntimeObject lVal = arg[0];
            BSRuntimeObject rVal = arg[1];

            if (lVal.TryConvertBool(out bool lD))
            {
                if (rVal.TryConvertBool(out bool rD))
                {
                    return new EngineRuntimeObject( lD || rD );
                }

                throw new Exception( "Invalid Operator Usage" );
            }

            throw new Exception( "Invalid Operator Usage" );
        }

        public static BSRuntimeObject OperatorXOr( BSRuntimeObject[] arg )
        {
            BSRuntimeObject lVal = arg[0];
            BSRuntimeObject rVal = arg[1];

            if (lVal.TryConvertBool(out bool lD))
            {
                if (rVal.TryConvertBool(out bool rD))
                {
                    return new EngineRuntimeObject(  lD   ^  rD  );
                }

                throw new Exception( "Invalid Operator Usage" );
            }

            throw new Exception( "Invalid Operator Usage" );
        }

        #endregion

    }

}
