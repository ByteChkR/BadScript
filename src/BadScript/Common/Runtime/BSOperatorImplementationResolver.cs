using System.Collections.Generic;
using System.Linq;
using BadScript.Common.Exceptions;
using BadScript.Common.Expressions.Implementations.Access;
using BadScript.Common.Expressions.Implementations.Value;
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
        public static bool AllowOperatorOverrides = true;

        private static readonly Dictionary<string, string> s_KeyMapping = new Dictionary<string, string>
        {
            { "++_Pre", "op_PreIncrement" },
            { "--_Pre", "op_PreDecrement" },
            { "++_Post", "op_PostIncrement" },
            { "--_Post", "op_PostDecrement" },
            { "+", "op_Plus" },
            { "-", "op_Minus" },
            { "*", "op_Multiply" },
            { "/", "op_Divide" },
            { "%", "op_Modulus" },
            { "+=", "op_SelfPlus" },
            { "-=", "op_SelfMinus" },
            { "*=", "op_SelfMultiply" },
            { "/=", "op_SelfDivide" },
            { "%=", "op_SelfModulus" },
            { "&&", "op_And" },
            { "||", "op_Or" },
            { "^", "op_XOr" },
            { "&=", "op_SelfAnd" },
            { "|=", "op_SelfOr" },
            { "^=", "op_SelfXOr" },
            { "==", "op_Equals" },
            { "!=", "op_InEqual" },
            { "<=", "op_LessOrEqual" },
            { ">=", "op_GreaterOrEqual" },
            { "<", "op_LessThan" },
            { ">", "op_GreaterThan" },
            { "!", "op_Not" },
            { "??", "op_NullCheck" },
            { "[]", "op_ArrayAccess" },
            { ".", "op_PropertyAccess" },
            { "()", "op_Invoke" },
        };

        private static List<ABSOperatorImplementation> m_Implementations;

        #region Public

        public static void AddImplementation(ABSOperatorImplementation o)
        {
            m_Implementations.Add(o);
        }

        public static ABSOperatorImplementation ResolveImplementation(string key, ABSObject[] args, bool allowOverrides = true)
        {
            ABSObject firstO = args.First();

            if (allowOverrides && AllowOperatorOverrides)
            {
                string opImplName = ResolveKey(key);
                if (firstO.HasProperty(opImplName))
                {
                    ABSObject impl = firstO.GetProperty(opImplName).ResolveReference();

                    if (impl is BSFunction fnc)
                    {
                        return new BSOperatorImplementation(key, fnc);
                    }

                    throw new BSRuntimeException($"Operator Implementation: '{opImplName}' is not a valid function.");
                }
            }

            ABSOperatorImplementation imp = m_Implementations.
                LastOrDefault(x => x.OperatorKey == key && x.IsCorrectImplementation(args));

            if (imp != null)
            {
                return imp;
            }

            throw new BSRuntimeException($"Could not find operator({key}) implementation");
        }

        #endregion

        #region Private

        static BSOperatorImplementationResolver()
        {
            m_Implementations = new List<ABSOperatorImplementation>();
            m_Implementations.Add(new BSAddOperatorImplementation());
            m_Implementations.Add(new BSAndOperatorImplementation());
            m_Implementations.Add(new BSDivideOperatorImplementation());
            m_Implementations.Add(new BSEqualityOperatorImplementation());
            m_Implementations.Add(new BSGreaterOrEqualOperatorImplementation());
            m_Implementations.Add(new BSGreaterThanOperatorImplementation());
            m_Implementations.Add(new BSInEqualityOperatorImplementation());
            m_Implementations.Add(new BSLessOrEqualOperatorImplementation());
            m_Implementations.Add(new BSLessThanOperatorImplementation());
            m_Implementations.Add(new BSMinusOperatorImplementation());
            m_Implementations.Add(new BSModuloOperatorImplementation());
            m_Implementations.Add(new BSMultiplyOperatorImplementation());
            m_Implementations.Add(new BSNotOperatorImplementation());
            m_Implementations.Add(new BSOrOperatorImplementation());
            m_Implementations.Add(new BSXOrOperatorImplementation());
            m_Implementations.Add(new BSNullTestOperatorImplementation());

            m_Implementations.Add(new BSSelfAddOperatorImplementation());
            m_Implementations.Add(new BSSelfMinusOperatorImplementation());
            m_Implementations.Add(new BSSelfDivideOperatorImplementation());
            m_Implementations.Add(new BSSelfMultiplyOperatorImplementation());
            m_Implementations.Add(new BSSelfModuloOperatorImplementation());

            m_Implementations.Add(new BSSelfAndOperatorImplementation());
            m_Implementations.Add(new BSSelfOrOperatorImplementation());
            m_Implementations.Add(new BSSelfXOrOperatorImplementation());

            m_Implementations.Add(new BSPrefixIncrementOperatorImplementation());
            m_Implementations.Add(new BSPostfixIncrementOperatorImplementation());
            m_Implementations.Add(new BSPrefixDecrementOperatorImplementation());
            m_Implementations.Add(new BSPostfixDecrementOperatorImplementation());

            m_Implementations.Add(new BSArrayAccessOperatorImplementation());
            m_Implementations.Add(new BSPropertyExpressionImplementation());
            m_Implementations.Add( new BSInvocationExpressionOperatorImplementation() );

        }

        public static string ResolveKey(string key)
        {
            return s_KeyMapping[key];
        }

        #endregion
    }

}
