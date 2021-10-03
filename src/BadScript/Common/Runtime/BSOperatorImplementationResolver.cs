using System.Collections.Generic;
using System.Linq;

using BadScript.Common.Exceptions;
using BadScript.Common.OperatorImplementations;
using BadScript.Common.OperatorImplementations.Implementations;
using BadScript.Common.OperatorImplementations.Implementations.Logic;
using BadScript.Common.OperatorImplementations.Implementations.Logic.Self;
using BadScript.Common.OperatorImplementations.Implementations.Math;
using BadScript.Common.OperatorImplementations.Implementations.Math.Self;
using BadScript.Common.OperatorImplementations.Implementations.Relational;
using BadScript.Common.Types;
using BadScript.Common.Types.References;

namespace BadScript.Common.Runtime
{

    public static class BSOperatorImplementationResolver
    {

        private static readonly Dictionary < string, string > s_KeyMapping = new Dictionary < string, string >
                                                                             {
                                                                                 { "++_Pre", "op_PreIncrement" },
                                                                                 { "--_Pre", "op_PreDecrement" },
                                                                                 { "++_Post", "op_PostIncrement" },
                                                                                 { "--_Post", "op_PostDecrement" },
                                                                                 { "+", "op_Addition" },
                                                                                 { "-", "op_Subtraction" },
                                                                                 { "*", "op_Multiply" },
                                                                                 { "/", "op_Division" },
                                                                                 { "%", "op_Modulus" },
                                                                                 { "+=", "op_SelfAddition" },
                                                                                 { "-=", "op_SelfSubtraction" },
                                                                                 { "*=", "op_SelfMultiply" },
                                                                                 { "/=", "op_SelfDivision" },
                                                                                 { "%=", "op_SelfModulus" },
                                                                                 { "&&", "op_And" },
                                                                                 { "||", "op_Or" },
                                                                                 { "^", "op_ExclusiveOr" },
                                                                                 { "&=", "op_SelfAnd" },
                                                                                 { "|=", "op_SelfOr" },
                                                                                 { "^=", "op_SelfExclusiveOr" },
                                                                                 { "==", "op_Equality" },
                                                                                 { "!=", "op_Inequality" },
                                                                                 { "<=", "op_LessThanOrEqual" },
                                                                                 { ">=", "op_GreaterThanOrEqual" },
                                                                                 { "<", "op_LessThan" },
                                                                                 { ">", "op_GreaterThan" },
                                                                                 { "!", "op_LogicalNot" },
                                                                                 { "??", "op_NullCheck" },
                                                                                 { "[]", "op_ArrayAccess" },
                                                                                 { ".", "op_PropertyAccess" },
                                                                                 { "()", "op_Invoke" },
                                                                             };

        private static List < ABSOperatorImplementation > m_Implementations;

        public static bool AllowOperatorOverrides { get; set; } = true;

        #region Public

        public static void AddImplementation(string functionName, ABSOperatorImplementation o )
        {
            s_KeyMapping[o.OperatorKey] = functionName;
            m_Implementations.Add( o );
        }

        public static ABSOperatorImplementation ResolveImplementation(
            string key,
            ABSObject[] args,
            bool allowOverrides = true )
        {
            ABSObject firstO = args.First();

            if ( allowOverrides && AllowOperatorOverrides )
            {
                string opImplName = ResolveKey( key );

                if ( firstO.HasProperty( opImplName ) )
                {
                    ABSObject impl = firstO.GetProperty( opImplName ).ResolveReference();

                    if ( impl is BSFunction fnc )
                    {
                        return new BSOperatorImplementation( key, fnc );
                    }

                    throw new BSRuntimeException( $"Operator Implementation: '{opImplName}' is not a valid function." );
                }
            }

            ABSOperatorImplementation imp = m_Implementations.
                LastOrDefault( x => x.OperatorKey == key );

            if ( imp != null && imp.IsCorrectImplementation( args ) )
            {
                return imp;
            }

            throw new BSRuntimeException( $"Could not find operator({key}) implementation" );
        }

        public static string ResolveKey( string key )
        {
            return s_KeyMapping[key];
        }

        #endregion

        #region Private

        static BSOperatorImplementationResolver()
        {
            m_Implementations = new List < ABSOperatorImplementation >();
            m_Implementations.Add( new BSAddOperatorImplementation() );
            m_Implementations.Add( new BSAndOperatorImplementation() );
            m_Implementations.Add( new BSDivideOperatorImplementation() );
            m_Implementations.Add( new BSEqualityOperatorImplementation() );
            m_Implementations.Add( new BSGreaterOrEqualOperatorImplementation() );
            m_Implementations.Add( new BSGreaterThanOperatorImplementation() );
            m_Implementations.Add( new BSInEqualityOperatorImplementation() );
            m_Implementations.Add( new BSLessOrEqualOperatorImplementation() );
            m_Implementations.Add( new BSLessThanOperatorImplementation() );
            m_Implementations.Add( new BSMinusOperatorImplementation() );
            m_Implementations.Add( new BSModuloOperatorImplementation() );
            m_Implementations.Add( new BSMultiplyOperatorImplementation() );
            m_Implementations.Add( new BSNotOperatorImplementation() );
            m_Implementations.Add( new BSOrOperatorImplementation() );
            m_Implementations.Add( new BSXOrOperatorImplementation() );
            m_Implementations.Add( new BSNullTestOperatorImplementation() );

            m_Implementations.Add( new BSSelfAddOperatorImplementation() );
            m_Implementations.Add( new BSSelfMinusOperatorImplementation() );
            m_Implementations.Add( new BSSelfDivideOperatorImplementation() );
            m_Implementations.Add( new BSSelfMultiplyOperatorImplementation() );
            m_Implementations.Add( new BSSelfModuloOperatorImplementation() );

            m_Implementations.Add( new BSSelfAndOperatorImplementation() );
            m_Implementations.Add( new BSSelfOrOperatorImplementation() );
            m_Implementations.Add( new BSSelfXOrOperatorImplementation() );

            m_Implementations.Add( new BSPrefixIncrementOperatorImplementation() );
            m_Implementations.Add( new BSPostfixIncrementOperatorImplementation() );
            m_Implementations.Add( new BSPrefixDecrementOperatorImplementation() );
            m_Implementations.Add( new BSPostfixDecrementOperatorImplementation() );

            m_Implementations.Add( new BSArrayAccessOperatorImplementation() );
            m_Implementations.Add( new BSPropertyExpressionImplementation() );
            m_Implementations.Add( new BSInvocationExpressionOperatorImplementation() );
        }

        #endregion

    }

}
