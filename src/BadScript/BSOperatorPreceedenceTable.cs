using System.Collections.Generic;
using System.Linq;
using BadScript.Common.Exceptions;
using BadScript.Common.Operators;
using BadScript.Common.Operators.Implementations;
using BadScript.Common.Runtime;
using BadScript.Common.Types;

namespace BadScript
{

    public static class BSOperatorPreceedenceTable
    {
        private static readonly List < BSOperator > s_Operators =
            new List < BSOperator >
            {
                new BSBinaryOperator(
                    6,
                    "+",
                    new BSFunction(
                        "function +(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "+", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    6,
                    "-",
                    new BSFunction(
                        "function -(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "-", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    5,
                    "*",
                    new BSFunction(
                        "function *(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "*", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    5,
                    "/",
                    new BSFunction(
                        "function /(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "/", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    5,
                    "%",
                    new BSFunction(
                        "function %(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "%", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    15,
                    "??",
                    new BSFunction(
                        "function ??(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "??", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    9,
                    "==",
                    new BSFunction(
                        "function ==(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "==", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    9,
                    "!=",
                    new BSFunction(
                        "function !=(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "!=", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    13,
                    "&&",
                    new BSFunction(
                        "function &&(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "&&", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    14,
                    "||",
                    new BSFunction(
                        "function ||(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "||", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    11,
                    "^",
                    new BSFunction(
                        "function ^(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "^", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    8,
                    "<",
                    new BSFunction(
                        "function <(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "<", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    8,
                    ">",
                    new BSFunction(
                        "function >(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( ">", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    8,
                    "<=",
                    new BSFunction(
                        "function <=(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( "<=", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSBinaryOperator(
                    8,
                    ">=",
                    new BSFunction(
                        "function >=(a, b)",
                        objects => BSOperatorImplementationResolver.
                                   ResolveImplementation( ">=", objects ).
                                   ExecuteOperator( objects ),
                        2
                    )
                ),
                new BSAssignmentOperator(),
                new BSMemberAccessOperator(),
            };

        private static readonly List < BSOperator > s_PrefixOperators =
            new List < BSOperator >
            {
                new BSUnaryOperator(
                    "!",
                    new BSFunction(
                        "function !(a)",
                        objects =>
                            BSOperatorImplementationResolver.
                                ResolveImplementation( "!", objects ).
                                ExecuteOperator( objects ),
                        1
                    )
                )
            };

        #region Public

        public static BSOperator Get( int p, string key )
        {
            return Get( s_Operators, p, key );
        }

        public static BSOperator GetPrefix( int p, string key )
        {
            return Get( s_PrefixOperators, p, key );
        }

        public static bool Has( int p, string k )
        {
            return s_Operators.Any( x => x.Preceedence <= p && x.OperatorKey == k );
        }

        #endregion

        #region Private

        private static BSOperator Get( List < BSOperator > ops, int p, string key )
        {
            foreach ( BSOperator bsOperator in ops )
            {
                if ( bsOperator.Preceedence <= p && bsOperator.OperatorKey == key )
                {
                    return bsOperator;
                }
            }

            throw new BSParserException( $"Can not Resolve Operator: {key} with precedence <= {p}" );
        }

        #endregion
    }

}
