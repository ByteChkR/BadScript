using System.Linq;

using BadScript.Exceptions;
using BadScript.Parser.Expressions;
using BadScript.Parser.OperatorImplementations;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Examples.SimpleCustomExpression
{

    public class BSJoinCustomOperatorImplementation : ABSOperatorImplementation
    {

        public BSJoinCustomOperatorImplementation() : base( "::" )
        {

        }

        public override bool IsCorrectImplementation( ABSObject[] args )
        {
            return args[0].ResolveReference() is ABSArray && args[1].ResolveReference() is ABSArray ||
                   args[0].ResolveReference() is ABSTable && args[1].ResolveReference() is ABSTable;
        }

        protected override ABSObject Execute( ABSObject[] args )
        {
            if ( args[0].ResolveReference() is ABSArray a1 &&
                 args[1].ResolveReference() is ABSArray a2)
            {
                return new BSArray(a1.Concat(a2));
            }
            else if ( args[0].ResolveReference() is ABSTable t1 &&
                      args[1].ResolveReference() is ABSTable t2)
            {
                ABSTable t = new BSTable( SourcePosition.Unknown );

                foreach ((ABSObject, ABSObject) pair in t1)
                {
                    t.InsertElement(pair.Item1, pair.Item2);
                }
                foreach ((ABSObject, ABSObject) pair in t2)
                {
                    t.InsertElement(pair.Item1, pair.Item2);
                }

                return t;
            }

            throw new BSInvalidTypeException(
                                             SourcePosition.Unknown,
                                             "Expected Table::Table or Array::Array",
                                             args[0],
                                             "Table",
                                             "Array"
                                            );
        }

    }

    

}