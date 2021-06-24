using System;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Math
{

    public class BSMathApi
    {
        #region Public

        public static void AddApi()
        {
            BSEngine.AddStatic( "math", GenerateMathApi() );
        }

        #endregion

        #region Private

        private static ABSTable GenerateMathApi()
        {
            BSTable ret = new BSTable();
            ret.InsertElement( new BSObject( "PI" ), new BSObject( ( decimal ) System.Math.PI ) );
            ret.InsertElement( new BSObject( "E" ), new BSObject( ( decimal ) System.Math.E ) );

            ret.InsertElement(
                new BSObject( "sin" ),
                new BSFunction(
                    "function sin(n)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        if ( o.TryConvertDecimal( out decimal path ) )
                        {
                            return new BSObject(
                                ( decimal ) System.Math.Sin( ( double ) path )
                            );
                        }

                        throw new Exception( "Expected Decimal" );
                    },
                    1
                )
            );

            ret.InsertElement(
                new BSObject( "cos" ),
                new BSFunction(
                    "function cos(n)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        if ( o.TryConvertDecimal( out decimal path ) )
                        {
                            return new BSObject(
                                ( decimal ) System.Math.Cos( ( double ) path )
                            );
                        }

                        throw new Exception( "Expected Decimal" );
                    },
                    1
                )
            );

            ret.InsertElement(
                new BSObject( "tan" ),
                new BSFunction(
                    "function tan(n)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        if ( o.TryConvertDecimal( out decimal path ) )
                        {
                            return new BSObject(
                                ( decimal ) System.Math.Tan( ( double ) path )
                            );
                        }

                        throw new Exception( "Expected Decimal" );
                    },
                    1
                )
            );

            ret.InsertElement(
                new BSObject( "sinh" ),
                new BSFunction(
                    "function sinh(n)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        if ( o.TryConvertDecimal( out decimal path ) )
                        {
                            return new BSObject(
                                ( decimal ) System.Math.Sinh( ( double ) path )
                            );
                        }

                        throw new Exception( "Expected Decimal" );
                    },
                    1
                )
            );

            ret.InsertElement(
                new BSObject( "cosh" ),
                new BSFunction(
                    "function cosh(n)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        if ( o.TryConvertDecimal( out decimal path ) )
                        {
                            return new BSObject(
                                ( decimal ) System.Math.Cosh( ( double ) path )
                            );
                        }

                        throw new Exception( "Expected Decimal" );
                    },
                    1
                )
            );

            ret.InsertElement(
                new BSObject( "tanh" ),
                new BSFunction(
                    "function tanh(n)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        if ( o.TryConvertDecimal( out decimal path ) )
                        {
                            return new BSObject(
                                ( decimal ) System.Math.Tanh( ( double ) path )
                            );
                        }

                        throw new Exception( "Expected Decimal" );
                    },
                    1
                )
            );

            ret.InsertElement(
                new BSObject( "asin" ),
                new BSFunction(
                    "function asin(n)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        if ( o.TryConvertDecimal( out decimal path ) )
                        {
                            return new BSObject(
                                ( decimal ) System.Math.Asin( ( double ) path )
                            );
                        }

                        throw new Exception( "Expected Decimal" );
                    },
                    1
                )
            );

            ret.InsertElement(
                new BSObject( "acos" ),
                new BSFunction(
                    "function acos(n)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        if ( o.TryConvertDecimal( out decimal path ) )
                        {
                            return new BSObject(
                                ( decimal ) System.Math.Acos( ( double ) path )
                            );
                        }

                        throw new Exception( "Expected Decimal" );
                    },
                    1
                )
            );

            ret.InsertElement(
                new BSObject( "atan" ),
                new BSFunction(
                    "function atan(n)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        if ( o.TryConvertDecimal( out decimal path ) )
                        {
                            return new BSObject(
                                ( decimal ) System.Math.Atan( ( double ) path )
                            );
                        }

                        throw new Exception( "Expected Decimal" );
                    },
                    1
                )
            );

            ret.InsertElement(
                new BSObject( "atan2" ),
                new BSFunction(
                    "function atan2(x, y)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        if ( o.TryConvertDecimal( out decimal x ) &&
                             args[1].
                                 ResolveReference().
                                 TryConvertDecimal( out decimal y ) )
                        {
                            return new BSObject(
                                ( decimal ) System.Math.Atan2(
                                    ( double ) x,
                                    ( double ) y
                                )
                            );
                        }

                        throw new Exception( "Expected Decimal" );
                    },
                    2
                )
            );

            ret.InsertElement(
                new BSObject( "log" ),
                new BSFunction(
                    "function log(n, newBase)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        if ( args.Length == 2 )
                        {
                            if ( o.TryConvertDecimal( out decimal path2 ) &&
                                 args[1].TryConvertDecimal( out decimal newB ) )
                            {
                                return new BSObject(
                                    ( decimal ) System.Math.Log(
                                        ( double ) path2,
                                        ( double ) newB
                                    )
                                );
                            }

                            throw new Exception( "Expected Decimal" );
                        }

                        if ( o.TryConvertDecimal( out decimal path1 ) )
                        {
                            return new BSObject(
                                ( decimal ) System.Math.Log( ( double ) path1 )
                            );
                        }

                        throw new Exception( "Expected Decimal" );
                    },
                    2
                )
            );

            ret.InsertElement(
                new BSObject( "abs" ),
                new BSFunction(
                    "function abs(n)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        if ( o.TryConvertDecimal( out decimal path1 ) )
                        {
                            return new BSObject(
                                ( decimal ) System.Math.Abs( ( double ) path1 )
                            );
                        }

                        throw new Exception( "Expected Decimal" );
                    },
                    1
                )
            );

            ret.InsertElement(
                new BSObject( "ceiling" ),
                new BSFunction(
                    "function ceiling(n)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        if ( o.TryConvertDecimal( out decimal path1 ) )
                        {
                            return new BSObject(
                                ( decimal ) System.Math.Ceiling( ( double ) path1 )
                            );
                        }

                        throw new Exception( "Expected Decimal" );
                    },
                    1
                )
            );

            ret.InsertElement(
                new BSObject( "floor" ),
                new BSFunction(
                    "function floor(n)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        if ( o.TryConvertDecimal( out decimal path1 ) )
                        {
                            return new BSObject(
                                ( decimal ) System.Math.Floor( ( double ) path1 )
                            );
                        }

                        throw new Exception( "Expected Decimal" );
                    },
                    1
                )
            );

            ret.InsertElement(
                new BSObject( "exp" ),
                new BSFunction(
                    "function exp(n)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        if ( o.TryConvertDecimal( out decimal path1 ) )
                        {
                            return new BSObject(
                                ( decimal ) System.Math.Exp( ( double ) path1 )
                            );
                        }

                        throw new Exception( "Expected Decimal" );
                    },
                    1
                )
            );

            ret.InsertElement(
                new BSObject( "log10" ),
                new BSFunction(
                    "function log10(n)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        if ( o.TryConvertDecimal( out decimal path1 ) )
                        {
                            return new BSObject(
                                ( decimal ) System.Math.Log10( ( double ) path1 )
                            );
                        }

                        throw new Exception( "Expected Decimal" );
                    },
                    1
                )
            );

            ret.InsertElement(
                new BSObject( "sign" ),
                new BSFunction(
                    "function sign(n)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        if ( o.TryConvertDecimal( out decimal path1 ) )
                        {
                            return new BSObject(
                                ( decimal ) System.Math.Sign( ( double ) path1 )
                            );
                        }

                        throw new Exception( "Expected Decimal" );
                    },
                    1
                )
            );

            ret.InsertElement(
                new BSObject( "min" ),
                new BSFunction(
                    "function min(x, y)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        if ( o.TryConvertDecimal( out decimal x ) &&
                             args[1].TryConvertDecimal( out decimal y ) )
                        {
                            return new BSObject(
                                ( decimal ) System.Math.Min(
                                    ( double ) x,
                                    ( double ) y
                                )
                            );
                        }

                        throw new Exception( "Expected Decimal" );
                    },
                    2
                )
            );

            ret.InsertElement(
                new BSObject( "max" ),
                new BSFunction(
                    "function max(x, y)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        if ( o.TryConvertDecimal( out decimal x ) &&
                             args[1].
                                 ResolveReference().
                                 TryConvertDecimal( out decimal y ) )
                        {
                            return new BSObject(
                                ( decimal ) System.Math.Max(
                                    ( double ) x,
                                    ( double ) y
                                )
                            );
                        }

                        throw new Exception( "Expected Decimal" );
                    },
                    1
                )
            );

            ret.InsertElement(
                new BSObject( "pow" ),
                new BSFunction(
                    "function pow(x, y)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        if ( o.TryConvertDecimal( out decimal x ) &&
                             args[1].
                                 ResolveReference().
                                 TryConvertDecimal( out decimal y ) )
                        {
                            return new BSObject(
                                ( decimal ) System.Math.Pow(
                                    ( double ) x,
                                    ( double ) y
                                )
                            );
                        }

                        throw new Exception( "Expected Decimal" );
                    },
                    1
                )
            );

            ret.InsertElement(
                new BSObject( "sqrt" ),
                new BSFunction(
                    "function sqrt(n)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        if ( o.TryConvertDecimal( out decimal x ) )
                        {
                            return new BSObject(
                                ( decimal ) System.Math.Sqrt( ( double ) x )
                            );
                        }

                        throw new Exception( "Expected Decimal" );
                    },
                    1
                )
            );

            ret.InsertElement(
                new BSObject( "truncate" ),
                new BSFunction(
                    "function truncate(n)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        if ( o.TryConvertDecimal( out decimal x ) )
                        {
                            return new BSObject(
                                ( decimal ) System.Math.Truncate( ( double ) x )
                            );
                        }

                        throw new Exception( "Expected Decimal" );
                    },
                    1
                )
            );

            return ret;
        }

        #endregion
    }

}
