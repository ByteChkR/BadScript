using System.Collections.Generic;
using System.Linq;

using BadScript.Exceptions;
using BadScript.Parser.Operators.Implementations;

namespace BadScript.Parser.Operators
{

    public static class BSOperatorPreceedenceTable
    {

        private static readonly List < BSOperator > s_Operators =
            new List < BSOperator >
            {
                new BSBinaryOperator(
                                     6,
                                     "+",
                                     "a,b",
                                     2
                                    ),
                new BSBinaryOperator(
                                     6,
                                     "-",
                                     "a,b",
                                     2
                                    ),
                new BSBinaryOperator(
                                     5,
                                     "*",
                                     "a,b",
                                     2
                                    ),
                new BSBinaryOperator(
                                     5,
                                     "/",
                                     "a,b",
                                     2
                                    ),
                new BSBinaryOperator(
                                     5,
                                     "%",
                                     "a,b",
                                     2
                                    ),
                new BSBinaryOperator(
                                     15,
                                     "??",
                                     "a,b",
                                     2
                                    ),
                new BSBinaryOperator(
                                     9,
                                     "==",
                                     "a,b",
                                     2
                                    ),
                new BSBinaryOperator(
                                     9,
                                     "!=",
                                     "a,b",
                                     2
                                    ),
                new BSBinaryOperator(
                                     13,
                                     "&&",
                                     "a,b",
                                     2
                                    ),
                new BSBinaryOperator(
                                     14,
                                     "||",
                                     "a,b",
                                     2
                                    ),
                new BSBinaryOperator(
                                     11,
                                     "^",
                                     "a,b",
                                     2
                                    ),
                new BSBinaryOperator(
                                     8,
                                     "<",
                                     "a,b",
                                     2
                                    ),
                new BSBinaryOperator(
                                     8,
                                     ">",
                                     "a,b",
                                     2
                                    ),
                new BSBinaryOperator(
                                     8,
                                     "<=",
                                     "a,b",
                                     2
                                    ),
                new BSBinaryOperator(
                                     8,
                                     ">=",
                                     "a,b",
                                     2
                                    ),
                new BSAssignmentOperator(),
                new BSMemberAccessOperator(),
                new BSNullCheckedMemberAccessOperator(),
                new BSBinaryOperator(
                                     6,
                                     "+=",
                                     "a,b",
                                     2
                                    ),
                new BSBinaryOperator(
                                     6,
                                     "-=",
                                     "a,b",
                                     2
                                    ),
                new BSBinaryOperator(
                                     5,
                                     "*=",
                                     "a,b",
                                     2
                                    ),
                new BSBinaryOperator(
                                     5,
                                     "/=",
                                     "a,b",
                                     2
                                    ),
                new BSBinaryOperator(
                                     5,
                                     "%=",
                                     "a,b",
                                     2
                                    ),
                new BSBinaryOperator(
                                     13,
                                     "&=",
                                     "a,b",
                                     2
                                    ),
                new BSBinaryOperator(
                                     14,
                                     "|=",
                                     "a,b",
                                     2
                                    ),
                new BSBinaryOperator(
                                     11,
                                     "^=",
                                     "a,b",
                                     2
                                    ),
                new BSBinaryOperator(
                                     3,
                                     "..",
                                     "a..b",
                                     2
                                    )
            };

        private static readonly List < BSOperator > s_PostfixOperators = new List < BSOperator >
                                                                         {
                                                                             new BSUnaryOperator(
                                                                                  "++",
                                                                                  "++_Post",
                                                                                  "a",
                                                                                  1
                                                                                 ),
                                                                             new BSUnaryOperator(
                                                                                  "--",
                                                                                  "--_Post",
                                                                                  "a",
                                                                                  1
                                                                                 )
                                                                         };

        private static readonly List < BSOperator > s_PrefixOperators =
            new List < BSOperator >
            {
                new BSUnaryOperator(
                                    "!",
                                    "!",
                                    "a",
                                    1
                                   ),
                new BSUnaryOperator(
                                    "++",
                                    "++_Pre",
                                    "a",
                                    1
                                   ),
                new BSUnaryOperator(
                                    "--",
                                    "--_Pre",
                                    "a",
                                    1
                                   )
            };

        #region Public

        public static BSOperator Get( int p, string key )
        {
            
            return Get( s_Operators, p, key );
        }

        public static BSOperator GetPostfix( int p, string key )
        {
            return Get( s_PostfixOperators, p, key );
        }

        public static BSOperator GetPrefix( int p, string key )
        {
            return Get( s_PrefixOperators, p, key );
        }

        public static bool Has( int p, string k )
        {
            return s_Operators.Any( x => x.Preceedence <= p && x.OperatorKey == k );
        }

        public static bool HasPostfix( int p, string k )
        {
            return s_PostfixOperators.Any( x => x.Preceedence <= p && x.OperatorKey == k );
        }

        public static bool HasPrefix( int p, string k )
        {
            return s_PrefixOperators.Any( x => x.Preceedence <= p && x.OperatorKey == k );
        }

        public static void Set( BSOperator op )
        {
            s_Operators.Add( op );
        }

        public static void SetPostfix( BSOperator op )
        {
            s_PostfixOperators.Add( op );
        }

        public static void SetPrefix( BSOperator op )
        {
            s_PrefixOperators.Add( op );
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
