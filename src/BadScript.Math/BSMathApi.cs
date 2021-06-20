using System;

using BadScript.Runtime;
using BadScript.Runtime.Implementations;

namespace BadScript.Apis.Math
{

    public class BSMathApi
    {

        public static void AddApi()
        {
            BSEngine.AddStatic( "math", GenerateMathApi() );
        }

        #region Public

        private static BSRuntimeTable GenerateMathApi()
        {
            EngineRuntimeTable ret = new EngineRuntimeTable();
            ret.InsertElement( new EngineRuntimeObject( "PI" ), new EngineRuntimeObject( ( decimal ) System.Math.PI ) );
            ret.InsertElement( new EngineRuntimeObject( "E" ), new EngineRuntimeObject( ( decimal ) System.Math.E ) );

            ret.InsertElement(
                              new EngineRuntimeObject( "sin" ),
                              new BSRuntimeFunction(
                                                    "function sin(n)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( o is BSRuntimeReference r )
                                                        {
                                                            o = r.Get();
                                                        }

                                                        if ( o.TryConvertDecimal( out decimal path ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) System.Math.Sin( ( double ) path )
                                                                );
                                                        }

                                                        throw new Exception( "Expected Decimal" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "cos" ),
                              new BSRuntimeFunction(
                                                    "function cos(n)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( o is BSRuntimeReference r )
                                                        {
                                                            o = r.Get();
                                                        }

                                                        if ( o.TryConvertDecimal( out decimal path ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) System.Math.Cos( ( double ) path )
                                                                );
                                                        }

                                                        throw new Exception( "Expected Decimal" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "tan" ),
                              new BSRuntimeFunction(
                                                    "function tan(n)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( o is BSRuntimeReference r )
                                                        {
                                                            o = r.Get();
                                                        }

                                                        if ( o.TryConvertDecimal( out decimal path ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) System.Math.Tan( ( double ) path )
                                                                );
                                                        }

                                                        throw new Exception( "Expected Decimal" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "sinh" ),
                              new BSRuntimeFunction(
                                                    "function sinh(n)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( o is BSRuntimeReference r )
                                                        {
                                                            o = r.Get();
                                                        }

                                                        if ( o.TryConvertDecimal( out decimal path ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) System.Math.Sinh( ( double ) path )
                                                                );
                                                        }

                                                        throw new Exception( "Expected Decimal" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "cosh" ),
                              new BSRuntimeFunction(
                                                    "function cosh(n)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( o is BSRuntimeReference r )
                                                        {
                                                            o = r.Get();
                                                        }

                                                        if ( o.TryConvertDecimal( out decimal path ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) System.Math.Cosh( ( double ) path )
                                                                );
                                                        }

                                                        throw new Exception( "Expected Decimal" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "tanh" ),
                              new BSRuntimeFunction(
                                                    "function tanh(n)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( o is BSRuntimeReference r )
                                                        {
                                                            o = r.Get();
                                                        }

                                                        if ( o.TryConvertDecimal( out decimal path ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) System.Math.Tanh( ( double ) path )
                                                                );
                                                        }

                                                        throw new Exception( "Expected Decimal" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "asin" ),
                              new BSRuntimeFunction(
                                                    "function asin(n)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( o is BSRuntimeReference r )
                                                        {
                                                            o = r.Get();
                                                        }

                                                        if ( o.TryConvertDecimal( out decimal path ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) System.Math.Asin( ( double ) path )
                                                                );
                                                        }

                                                        throw new Exception( "Expected Decimal" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "acos" ),
                              new BSRuntimeFunction(
                                                    "function acos(n)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( o is BSRuntimeReference r )
                                                        {
                                                            o = r.Get();
                                                        }

                                                        if ( o.TryConvertDecimal( out decimal path ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) System.Math.Acos( ( double ) path )
                                                                );
                                                        }

                                                        throw new Exception( "Expected Decimal" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "atan" ),
                              new BSRuntimeFunction(
                                                    "function atan(n)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( o is BSRuntimeReference r )
                                                        {
                                                            o = r.Get();
                                                        }

                                                        if ( o.TryConvertDecimal( out decimal path ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) System.Math.Atan( ( double ) path )
                                                                );
                                                        }

                                                        throw new Exception( "Expected Decimal" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "atan2" ),
                              new BSRuntimeFunction(
                                                    "function atan2(x, y)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( o is BSRuntimeReference r2 )
                                                        {
                                                            o = r2.Get();
                                                        }

                                                        if ( o.TryConvertDecimal( out decimal x ) &&
                                                             args[1].TryConvertDecimal( out decimal y ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) System.Math.Atan2(
                                                                      ( double ) x,
                                                                      ( double ) y
                                                                     )
                                                                );
                                                        }

                                                        throw new Exception( "Expected Decimal" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "log" ),
                              new BSRuntimeFunction(
                                                    "function log(n, newBase)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( args.Length == 2 )
                                                        {
                                                            if ( o is BSRuntimeReference r2 )
                                                            {
                                                                o = r2.Get();
                                                            }

                                                            if ( o.TryConvertDecimal( out decimal path2 ) &&
                                                                 args[1].TryConvertDecimal( out decimal newB ) )
                                                            {
                                                                return new EngineRuntimeObject(
                                                                     ( decimal ) System.Math.Log(
                                                                          ( double ) path2,
                                                                          ( double ) newB
                                                                         )
                                                                    );
                                                            }

                                                            throw new Exception( "Expected Decimal" );
                                                        }

                                                        if ( o is BSRuntimeReference r1 )
                                                        {
                                                            o = r1.Get();
                                                        }

                                                        if ( o.TryConvertDecimal( out decimal path1 ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) System.Math.Log( ( double ) path1 )
                                                                );
                                                        }

                                                        throw new Exception( "Expected Decimal" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "abs" ),
                              new BSRuntimeFunction(
                                                    "function abs(n)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( o is BSRuntimeReference r1 )
                                                        {
                                                            o = r1.Get();
                                                        }

                                                        if ( o.TryConvertDecimal( out decimal path1 ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) System.Math.Abs( ( double ) path1 )
                                                                );
                                                        }

                                                        throw new Exception( "Expected Decimal" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "ceiling" ),
                              new BSRuntimeFunction(
                                                    "function ceiling(n)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( o is BSRuntimeReference r1 )
                                                        {
                                                            o = r1.Get();
                                                        }

                                                        if ( o.TryConvertDecimal( out decimal path1 ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) System.Math.Ceiling( ( double ) path1 )
                                                                );
                                                        }

                                                        throw new Exception( "Expected Decimal" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "floor" ),
                              new BSRuntimeFunction(
                                                    "function floor(n)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( o is BSRuntimeReference r1 )
                                                        {
                                                            o = r1.Get();
                                                        }

                                                        if ( o.TryConvertDecimal( out decimal path1 ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) System.Math.Floor( ( double ) path1 )
                                                                );
                                                        }

                                                        throw new Exception( "Expected Decimal" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "exp" ),
                              new BSRuntimeFunction(
                                                    "function exp(n)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( o is BSRuntimeReference r1 )
                                                        {
                                                            o = r1.Get();
                                                        }

                                                        if ( o.TryConvertDecimal( out decimal path1 ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) System.Math.Exp( ( double ) path1 )
                                                                );
                                                        }

                                                        throw new Exception( "Expected Decimal" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "log10" ),
                              new BSRuntimeFunction(
                                                    "function log10(n)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( o is BSRuntimeReference r1 )
                                                        {
                                                            o = r1.Get();
                                                        }

                                                        if ( o.TryConvertDecimal( out decimal path1 ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) System.Math.Log10( ( double ) path1 )
                                                                );
                                                        }

                                                        throw new Exception( "Expected Decimal" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "sign" ),
                              new BSRuntimeFunction(
                                                    "function sign(n)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( o is BSRuntimeReference r1 )
                                                        {
                                                            o = r1.Get();
                                                        }

                                                        if ( o.TryConvertDecimal( out decimal path1 ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) System.Math.Sign( ( double ) path1 )
                                                                );
                                                        }

                                                        throw new Exception( "Expected Decimal" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "min" ),
                              new BSRuntimeFunction(
                                                    "function min(x, y)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( o is BSRuntimeReference r2 )
                                                        {
                                                            o = r2.Get();
                                                        }

                                                        if ( o.TryConvertDecimal( out decimal x ) &&
                                                             args[1].TryConvertDecimal( out decimal y ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) System.Math.Min(
                                                                      ( double ) x,
                                                                      ( double ) y
                                                                     )
                                                                );
                                                        }

                                                        throw new Exception( "Expected Decimal" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "max" ),
                              new BSRuntimeFunction(
                                                    "function max(x, y)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( o is BSRuntimeReference r2 )
                                                        {
                                                            o = r2.Get();
                                                        }

                                                        if ( o.TryConvertDecimal( out decimal x ) &&
                                                             args[1].TryConvertDecimal( out decimal y ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) System.Math.Max(
                                                                      ( double ) x,
                                                                      ( double ) y
                                                                     )
                                                                );
                                                        }

                                                        throw new Exception( "Expected Decimal" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "pow" ),
                              new BSRuntimeFunction(
                                                    "function pow(x, y)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( o is BSRuntimeReference r2 )
                                                        {
                                                            o = r2.Get();
                                                        }

                                                        if ( o.TryConvertDecimal( out decimal x ) &&
                                                             args[1].TryConvertDecimal( out decimal y ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) System.Math.Pow(
                                                                      ( double ) x,
                                                                      ( double ) y
                                                                     )
                                                                );
                                                        }

                                                        throw new Exception( "Expected Decimal" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "sqrt" ),
                              new BSRuntimeFunction(
                                                    "function sqrt(n)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( o is BSRuntimeReference r2 )
                                                        {
                                                            o = r2.Get();
                                                        }

                                                        if ( o.TryConvertDecimal( out decimal x ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) System.Math.Sqrt( ( double ) x )
                                                                );
                                                        }

                                                        throw new Exception( "Expected Decimal" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "truncate" ),
                              new BSRuntimeFunction(
                                                    "function truncate(n)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( o is BSRuntimeReference r2 )
                                                        {
                                                            o = r2.Get();
                                                        }

                                                        if ( o.TryConvertDecimal( out decimal x ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) System.Math.Truncate( ( double ) x )
                                                                );
                                                        }

                                                        throw new Exception( "Expected Decimal" );
                                                    }
                                                   )
                             );

            return ret;
        }

        #endregion

    }

}
