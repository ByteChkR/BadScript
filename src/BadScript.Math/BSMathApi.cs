using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;
using BadScript.Interfaces;

namespace BadScript.Math
{

    public class BSMathApi : ABSScriptInterface
    {

        #region Public

        public BSMathApi() : base( "math" )
        {
        }

        public override void AddApi( ABSTable root )
        {
            GenerateMathApi( root );
        }

        #endregion

        #region Private

        private static void GenerateMathApi( ABSTable ret )
        {
            ret.InsertElement( new BSObject( "PI" ), new BSObject( ( decimal )System.Math.PI ) );
            ret.InsertElement( new BSObject( "E" ), new BSObject( ( decimal )System.Math.E ) );

            ret.InsertElement(
                              new BSObject( "sin" ),
                              new BSFunction(
                                             "function sin(n)",
                                             args =>
                                             {
                                                 ABSObject o = args[0].ResolveReference();

                                                 return new BSObject(
                                                                     ( decimal )System.Math.Sin(
                                                                          ( double )o.ConvertDecimal()
                                                                         )
                                                                    );
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

                                                 return new BSObject(
                                                                     ( decimal )System.Math.Cos(
                                                                          ( double )o.ConvertDecimal()
                                                                         )
                                                                    );
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

                                                 return new BSObject(
                                                                     ( decimal )System.Math.Tan(
                                                                          ( double )o.ConvertDecimal()
                                                                         )
                                                                    );
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

                                                 return new BSObject(
                                                                     ( decimal )System.Math.Sinh(
                                                                          ( double )o.ConvertDecimal()
                                                                         )
                                                                    );
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

                                                 return new BSObject(
                                                                     ( decimal )System.Math.Cosh(
                                                                          ( double )o.ConvertDecimal()
                                                                         )
                                                                    );
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

                                                 return new BSObject(
                                                                     ( decimal )System.Math.Tanh(
                                                                          ( double )o.ConvertDecimal()
                                                                         )
                                                                    );
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

                                                 return new BSObject(
                                                                     ( decimal )System.Math.Asin(
                                                                          ( double )o.ConvertDecimal()
                                                                         )
                                                                    );
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

                                                 return new BSObject(
                                                                     ( decimal )System.Math.Acos(
                                                                          ( double )o.ConvertDecimal()
                                                                         )
                                                                    );
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

                                                 return new BSObject(
                                                                     ( decimal )System.Math.Atan(
                                                                          ( double )o.ConvertDecimal()
                                                                         )
                                                                    );
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

                                                 return new BSObject(
                                                                     ( decimal )System.Math.Atan2(
                                                                          ( double )o.ConvertDecimal(),
                                                                          ( double )args[1].
                                                                              ResolveReference().
                                                                              ConvertDecimal()
                                                                         )
                                                                    );
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
                                                     return new BSObject(
                                                                         ( decimal )System.Math.Log(
                                                                              ( double )o.ConvertDecimal(),
                                                                              ( double )args[1].
                                                                                  ResolveReference().
                                                                                  ConvertDecimal()
                                                                             )
                                                                        );
                                                 }

                                                 return new BSObject(
                                                                     ( decimal )System.Math.Log(
                                                                          ( double )o.ConvertDecimal()
                                                                         )
                                                                    );
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

                                                 return new BSObject(
                                                                     ( decimal )System.Math.Abs(
                                                                          ( double )o.ConvertDecimal()
                                                                         )
                                                                    );
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

                                                 return new BSObject(
                                                                     ( decimal )System.Math.Ceiling(
                                                                          ( double )o.ConvertDecimal()
                                                                         )
                                                                    );
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

                                                 return new BSObject(
                                                                     ( decimal )System.Math.Floor(
                                                                          ( double )o.ConvertDecimal()
                                                                         )
                                                                    );
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

                                                 return new BSObject(
                                                                     ( decimal )System.Math.Exp(
                                                                          ( double )o.ConvertDecimal()
                                                                         )
                                                                    );
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

                                                 return new BSObject(
                                                                     ( decimal )System.Math.Log10(
                                                                          ( double )o.ConvertDecimal()
                                                                         )
                                                                    );
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

                                                 return new BSObject(
                                                                     ( decimal )System.Math.Sign(
                                                                          ( double )o.ConvertDecimal()
                                                                         )
                                                                    );
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
                                                 return new BSObject(
                                                                     ( decimal )System.Math.Min(
                                                                          ( double )args[0].
                                                                              ResolveReference().
                                                                              ConvertDecimal(),
                                                                          ( double )args[1].
                                                                              ResolveReference().
                                                                              ConvertDecimal()
                                                                         )
                                                                    );
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
                                                 return new BSObject(
                                                                     ( decimal )System.Math.Max(
                                                                          ( double )args[0].
                                                                              ResolveReference().
                                                                              ConvertDecimal(),
                                                                          ( double )args[1].
                                                                              ResolveReference().
                                                                              ConvertDecimal()
                                                                         )
                                                                    );
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
                                                 return new BSObject(
                                                                     ( decimal )System.Math.Pow(
                                                                          ( double )args[0].
                                                                              ResolveReference().
                                                                              ConvertDecimal(),
                                                                          ( double )args[1].
                                                                              ResolveReference().
                                                                              ConvertDecimal()
                                                                         )
                                                                    );
                                             },
                                             2
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "sqrt" ),
                              new BSFunction(
                                             "function sqrt(n)",
                                             args =>
                                             {
                                                 ABSObject o = args[0].ResolveReference();

                                                 return new BSObject(
                                                                     ( decimal )System.Math.Sqrt(
                                                                          ( double )o.ConvertDecimal()
                                                                         )
                                                                    );
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

                                                 return new BSObject(
                                                                     ( decimal )System.Math.Truncate(
                                                                          ( double )o.ConvertDecimal()
                                                                         )
                                                                    );
                                             },
                                             1
                                            )
                             );

            ret.InsertElement( new BSObject( "isPrime" ), new BSFunction( "function isPrime(num)", PrimeTest, 1 ) );
        }

        private static ABSObject PrimeTest( ABSObject[] arg )
        {
            int n = ( int )arg[0].ConvertDecimal();

            if ( n <= 1 )
            {
                return BSObject.False;
            }

            for ( int i = 2; i <= System.Math.Sqrt( n ); i++ )
            {
                if ( n % i == 0 )
                {
                    return BSObject.False;
                }
            }

            return BSObject.True;
        }

        #endregion

    }

}
