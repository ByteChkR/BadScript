using BadScript.Exceptions;
using BadScript.Parser.Expressions;
using BadScript.Scopes;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Interfaces.Collection
{

    public class BSCollectionInterface : ABSScriptInterface
    {

        public BSCollectionInterface(  ) : base( "Collection" )
        {
        }

        public override void AddApi( ABSTable root )
        {
            root.InsertElement(
                                  new BSObject("Size"),
                                  new BSFunction(
                                                 "function Size(table/array)",
                                                 (args) =>
                                                 {
                                                     ABSObject arg = args[0].ResolveReference();

                                                     if (arg is ABSArray ar)
                                                     {
                                                         return new BSObject((decimal)ar.GetLength());
                                                     }

                                                     if (arg is ABSTable t)
                                                     {
                                                         return new BSObject((decimal)t.GetLength());
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
                               new BSObject("Keys"),
                               new BSFunction(
                                              "function Keys(table)",
                                              objects =>
                                              {
                                                  ABSObject a = objects[0].ResolveReference();

                                                  if (a is ABSTable t)
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
                               new BSObject("Values"),
                               new BSFunction(
                                              "function Values(table)",
                                              objects =>
                                              {
                                                  ABSObject a = objects[0].ResolveReference();

                                                  if (a is ABSTable t)
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
                               new BSObject("Lock"),
                               new BSFunction(
                                              "function Lock(array/table)",
                                              (args) =>
                                              {
                                                  ABSObject arg = args[0].ResolveReference();

                                                  if (arg is BSArray arr)
                                                  {
                                                      arr.Lock();
                                                  }
                                                  else if (arg is BSTable table)
                                                  {
                                                      table.Lock();
                                                  }

                                                  return BSObject.Null;
                                              },
                                              1
                                             )
                              );

            root.InsertElement(
                               new BSObject("HasKey"),
                               new BSFunction(
                                              "function HasKey(table, key)",
                                              (args) =>
                                              {
                                                  ABSObject arg = args[0].ResolveReference();

                                                  if (arg is ABSTable table)
                                                  {
                                                      return table.HasElement(
                                                                              args[1].ResolveReference()
                                                                             )
                                                                 ? BSObject.True
                                                                 : BSObject.False
                                                          ;
                                                  }
                                                  else if (arg is IBSWrappedObject wo)
                                                  {
                                                      object o = wo.GetInternalObject();

                                                      if (o is BSScope scope)
                                                      {
                                                          return scope.Has(args[1].ConvertString())
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

        }

    }

}
