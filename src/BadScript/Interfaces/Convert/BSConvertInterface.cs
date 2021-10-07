using System.Collections.Generic;
using System.Linq;

using BadScript.Exceptions;
using BadScript.Parser.Expressions;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.Implementations.Types;
using BadScript.Types.References;

namespace BadScript.Interfaces.Convert
{

    public class BSConvertInterface : ABSScriptInterface
    {

        #region Public

        public BSConvertInterface() : base( "Convert" )
        {
        }

        public override void AddApi( ABSTable root )
        {
            AddIsFunctions( root );
            AddToFunctions( root );
        }

        #endregion

        #region Private

        private void AddIsFunctions( ABSTable root )
        {
            root.InsertElement(
                               new BSObject( "IsTable" ),
                               new BSFunction(
                                              "function IsTable(obj)",
                                              ( args ) =>
                                              {
                                                  ABSObject arg = args[0].ResolveReference();

                                                  if ( arg is ABSTable )
                                                  {
                                                      return BSObject.True;
                                                  }

                                                  return BSObject.False;
                                              },
                                              1
                                             )
                              );

            root.InsertElement(
                               new BSObject( "IsType" ),
                               new BSFunction(
                                              "function IsType(obj)",
                                              ( args ) =>
                                              {
                                                  ABSObject arg = args[0].ResolveReference();

                                                  if ( arg is BSClassInstance )
                                                  {
                                                      return BSObject.True;
                                                  }

                                                  return BSObject.False;
                                              },
                                              1
                                             )
                              );

            root.InsertElement(
                               new BSObject( "IsLiteral" ),
                               new BSFunction(
                                              "function IsLiteral(obj)",
                                              ( args ) =>
                                              {
                                                  ABSObject arg = args[0].ResolveReference();

                                                  if ( arg.TryConvertString( out string _ ) ||
                                                       arg.TryConvertDecimal( out decimal _ ) ||
                                                       arg.TryConvertBool( out bool _ ) )
                                                  {
                                                      return BSObject.True;
                                                  }

                                                  return BSObject.False;
                                              },
                                              1
                                             )
                              );

            root.InsertElement(
                               new BSObject( "IsString" ),
                               new BSFunction(
                                              "function IsString(obj)",
                                              ( args ) =>
                                              {
                                                  ABSObject arg = args[0].ResolveReference();

                                                  if ( arg.TryConvertString( out string _ ) )
                                                  {
                                                      return BSObject.True;
                                                  }

                                                  return BSObject.False;
                                              },
                                              1
                                             )
                              );

            root.InsertElement(
                               new BSObject( "IsNumber" ),
                               new BSFunction(
                                              "function IsNumber(obj)",
                                              ( args ) =>
                                              {
                                                  ABSObject arg = args[0].ResolveReference();

                                                  if ( arg.TryConvertDecimal( out decimal _ ) )
                                                  {
                                                      return BSObject.True;
                                                  }

                                                  return BSObject.False;
                                              },
                                              1
                                             )
                              );

            root.InsertElement(
                               new BSObject( "IsBoolean" ),
                               new BSFunction(
                                              "function IsBoolean(obj)",
                                              ( args ) =>
                                              {
                                                  ABSObject arg = args[0].ResolveReference();

                                                  if ( arg.TryConvertBool( out bool _ ) )
                                                  {
                                                      return BSObject.True;
                                                  }

                                                  return BSObject.False;
                                              },
                                              1
                                             )
                              );

            root.InsertElement(
                               new BSObject( "IsArray" ),
                               new BSFunction(
                                              "function IsArray(obj)",
                                              ( args ) =>
                                              {
                                                  ABSObject arg = args[0].ResolveReference();

                                                  if ( arg is ABSArray )
                                                  {
                                                      return BSObject.True;
                                                  }

                                                  return BSObject.False;
                                              },
                                              1
                                             )
                              );

            root.InsertElement(
                               new BSObject( "IsFunction" ),
                               new BSFunction(
                                              "function IsFunction(obj)",
                                              ( args ) =>
                                              {
                                                  ABSObject arg = args[0].ResolveReference();

                                                  if ( arg is BSFunction )
                                                  {
                                                      return BSObject.True;
                                                  }

                                                  return BSObject.False;
                                              },
                                              1
                                             )
                              );

            root.InsertElement(
                               new BSObject( "Base64" ),
                               new BSTable(
                                           SourcePosition.Unknown,
                                           new Dictionary < ABSObject, ABSObject >
                                           {
                                               {
                                                   new BSObject( "To" ),
                                                   new BSFunction( "function To(dataArray)", ToBase64, 1 )
                                               },
                                               {
                                                   new BSObject( "From" ),
                                                   new BSFunction( "function From(str)", FromBase64, 1 )
                                               },
                                           }
                                          )
                              );
        }

        private void AddToFunctions( ABSTable root )
        {
            root.InsertElement(
                               "ToNumber",
                               new BSFunction(
                                              "function ToNumber(str)",
                                              x => new BSObject( decimal.Parse( x[0].ConvertString().Trim() ) ),
                                              1,
                                              1
                                             )
                              );

            root.InsertElement(
                               "ToBoolean",
                               new BSFunction(
                                              "function ToBoolean(str)",
                                              x => new BSObject( bool.Parse( x[0].ConvertString().Trim() ) ),
                                              1,
                                              1
                                             )
                              );

            root.InsertElement(
                               "ToString",
                               new BSFunction(
                                              "function ToString(obj)",
                                              x => new BSObject( x[0].ResolveReference().ToString() ),
                                              1,
                                              1
                                             )
                              );
        }

        private ABSObject FromBase64( ABSObject[] arg )
        {
            string str = arg[0].ConvertString();
            byte[] data = System.Convert.FromBase64String( str );

            return new BSArray( data.Select( x => new BSObject( ( decimal )x ) ) );
        }

        private ABSObject ToBase64( ABSObject[] arg )
        {
            if ( arg[0].ResolveReference() is ABSArray a )
            {
                byte[] data = new byte[a.GetLength()];

                for ( int i = 0; i < data.Length; i++ )
                {
                    data[i] = ( byte )a.GetElement( i ).ConvertDecimal();
                }

                return new BSObject( System.Convert.ToBase64String( data ) );
            }

            throw new BSRuntimeException( "Invalid Type. Expected Array of Numbers" );
        }

        #endregion

    }

}
