using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using BadScript.Exceptions;
using BadScript.Interfaces;
using BadScript.Parser.Expressions;
using BadScript.Scopes;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.Implementations.Types;
using BadScript.Types.References;

namespace BadScript.Core
{

    public class BSCoreInterface : ABSScriptInterface
    {

        #region Public

        public BSCoreInterface() : base( "Core" )
        {
        }

        public override void AddApi( ABSTable root )
        {
            root.InsertElement(
                               new BSObject( "Size" ),
                               new BSFunction(
                                              "function Size(table/array)",
                                              ( args ) =>
                                              {
                                                  ABSObject arg = args[0].ResolveReference();

                                                  if ( arg is ABSArray ar )
                                                  {
                                                      return new BSObject( ( decimal )ar.GetLength() );
                                                  }

                                                  if ( arg is ABSTable t )
                                                  {
                                                      return new BSObject( ( decimal )t.GetLength() );
                                                  }

                                                  throw new BSInvalidTypeException(
                                                       arg.Position,
                                                       "Can not get Size of object",
                                                       arg,
                                                       "Table",
                                                       "Array"
                                                      );
                                              },
                                              1
                                             )
                              );

            root.InsertElement(
                               new BSObject( "Keys" ),
                               new BSFunction(
                                              "function Keys(table)",
                                              objects =>
                                              {
                                                  ABSObject a = objects[0].ResolveReference();

                                                  if ( a is ABSTable t )
                                                  {
                                                      return t.Keys;
                                                  }

                                                  throw new BSInvalidTypeException(
                                                       a.Position,
                                                       "Object is not a table",
                                                       a,
                                                       "Table"
                                                      );
                                              },
                                              1
                                             )
                              );

            root.InsertElement(
                               new BSObject( "Values" ),
                               new BSFunction(
                                              "function Values(table)",
                                              objects =>
                                              {
                                                  ABSObject a = objects[0].ResolveReference();

                                                  if ( a is ABSTable t )
                                                  {
                                                      return t.Values;
                                                  }

                                                  throw new BSInvalidTypeException(
                                                       a.Position,
                                                       "Object is not a table",
                                                       a,
                                                       "Table"
                                                      );
                                              },
                                              1
                                             )
                              );

            root.InsertElement(
                               new BSObject( "Error" ),
                               new BSFunction(
                                              "function Error(obj)",
                                              ( args ) =>
                                              {
                                                  ABSObject arg = args[0].ResolveReference();

                                                  throw new BSRuntimeException( arg.Position, arg.ToString() );
                                              },
                                              1
                                             )
                              );

            root.InsertElement(
                               new BSObject( "Debug" ),
                               new BSFunction(
                                              "function Debug(obj)",
                                              ( args ) =>
                                              {
                                                  ABSObject arg = args[0].ResolveReference();

                                                  return new BSObject(
                                                                      arg.SafeToString(
                                                                           new Dictionary < ABSObject, string >()
                                                                          )
                                                                     );
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
                               new BSObject("IsFunction"),
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
                               new BSObject( "Lock" ),
                               new BSFunction(
                                              "function Lock(array/table)",
                                              ( args ) =>
                                              {
                                                  ABSObject arg = args[0].ResolveReference();

                                                  if ( arg is BSArray arr )
                                                  {
                                                      arr.Lock();
                                                  }
                                                  else if ( arg is BSTable table )
                                                  {
                                                      table.Lock();
                                                  }

                                                  return BSObject.Null;
                                              },
                                              1
                                             )
                              );

            root.InsertElement(
                               new BSObject( "HasKey" ),
                               new BSFunction(
                                              "function HasKey(table, key)",
                                              ( args ) =>
                                              {
                                                  ABSObject arg = args[0].ResolveReference();

                                                  if ( arg is ABSTable table )
                                                  {
                                                      return table.HasElement(
                                                                              args[1].ResolveReference()
                                                                             )
                                                                 ? BSObject.True
                                                                 : BSObject.False
                                                          ;
                                                  }
                                                  else if ( arg is IBSWrappedObject wo )
                                                  {
                                                      object o = wo.GetInternalObject();

                                                      if ( o is BSScope scope )
                                                      {
                                                          return scope.Has( args[1].ConvertString() )
                                                                     ? BSObject.True
                                                                     : BSObject.False;
                                                      }
                                                  }

                                                  throw new BSInvalidTypeException(
                                                       SourcePosition.Unknown,
                                                       "Expected Table",
                                                       arg,
                                                       "Table",
                                                       "Scope"
                                                      );
                                              },
                                              2
                                             )
                              );

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

            root.InsertElement( new BSObject( "Escape" ), new BSFunction( "function Escape(str)", EscapeString, 1 ) );

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

            root.InsertElement(
                               new BSObject( "Sleep" ),
                               new BSFunction(
                                              "function Sleep(ms)",
                                              ( args ) =>
                                              {
                                                  if ( args[0].TryConvertDecimal( out decimal lD ) )
                                                  {
                                                      Thread.Sleep( ( int )lD );

                                                      return BSObject.Null;
                                                  }

                                                  throw new BSInvalidTypeException(
                                                       args[0].Position,
                                                       "Invalid Sleep Time",
                                                       args[0],
                                                       "number"
                                                      );
                                              },
                                              1
                                             )
                              );
        }

        #endregion

        #region Private

        private ABSObject EscapeString( ABSObject[] arg )
        {
            string str = arg[0].ConvertString();

            return new BSObject( Uri.EscapeDataString( str ) );
        }

        private ABSObject FromBase64( ABSObject[] arg )
        {
            string str = arg[0].ConvertString();
            byte[] data = Convert.FromBase64String( str );

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

                return new BSObject( Convert.ToBase64String( data ) );
            }

            throw new BSRuntimeException( "Invalid Type. Expected Array of Numbers" );
        }

        #endregion

    }

}
