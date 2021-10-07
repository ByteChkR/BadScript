using BadScript.Interfaces;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Math
{

    public class BSMathInterface : ABSScriptInterface
    {

        #region Public

        public BSMathInterface() : base( "Math" )
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
                              new BSObject( "Sin" ),
                              new BSFunction(
                                             "function Sin(n)",
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
                              new BSObject( "Cos" ),
                              new BSFunction(
                                             "function Cos(n)",
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
                              new BSObject( "Tan" ),
                              new BSFunction(
                                             "function Tan(n)",
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
                              new BSObject( "Sinh" ),
                              new BSFunction(
                                             "function Sinh(n)",
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
                              new BSObject( "Cosh" ),
                              new BSFunction(
                                             "function Cosh(n)",
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
                              new BSObject( "Tanh" ),
                              new BSFunction(
                                             "function Tanh(n)",
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
                              new BSObject( "Asin" ),
                              new BSFunction(
                                             "function Asin(n)",
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
                              new BSObject( "Acos" ),
                              new BSFunction(
                                             "function Acos(n)",
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
                              new BSObject( "Atan" ),
                              new BSFunction(
                                             "function Atan(n)",
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
                              new BSObject( "Atan2" ),
                              new BSFunction(
                                             "function Atan2(x, y)",
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
                              new BSObject( "Log" ),
                              new BSFunction(
                                             "function Log(n, newBase)",
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
                              new BSObject( "Abs" ),
                              new BSFunction(
                                             "function Abs(n)",
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
                              new BSObject( "Ceiling" ),
                              new BSFunction(
                                             "function Ceiling(n)",
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
                              new BSObject( "Floor" ),
                              new BSFunction(
                                             "function Floor(n)",
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
                              new BSObject( "Exp" ),
                              new BSFunction(
                                             "function Exp(n)",
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
                              new BSObject( "Log10" ),
                              new BSFunction(
                                             "function Log10(n)",
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
                              new BSObject( "Sign" ),
                              new BSFunction(
                                             "function Sign(n)",
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
                              new BSObject( "Min" ),
                              new BSFunction(
                                             "function Min(x, y)",
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
                              new BSObject( "Max" ),
                              new BSFunction(
                                             "function Max(x, y)",
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
                              new BSObject( "Pow" ),
                              new BSFunction(
                                             "function Pow(x, y)",
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
                              new BSObject( "Sqrt" ),
                              new BSFunction(
                                             "function Sqrt(n)",
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
                              new BSObject( "Truncate" ),
                              new BSFunction(
                                             "function Truncate(n)",
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

            ret.InsertElement( new BSObject( "IsPrime" ), new BSFunction( "function IsPrime(num)", PrimeTest, 1 ) );
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
