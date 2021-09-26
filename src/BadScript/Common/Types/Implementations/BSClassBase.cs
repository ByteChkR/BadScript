using System.Collections.Generic;

using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Value;
using BadScript.Common.Runtime;

namespace BadScript.Common.Types.Implementations
{

    public static class BSClassBase
    {

        private static readonly Dictionary < string, BSClassExpression > m_Classes =
            new Dictionary < string, BSClassExpression >();

        private static BSClassInstance CreateBaseInstance( string name, BSScope scope )
        {
            BSClassExpression expr = m_Classes[name];
            BSClassInstance baseInstance = null;
            if ( expr.BaseName != null )
            {
                baseInstance = CreateBaseInstance(expr.BaseName, scope);
                scope = new BSScope(BSScopeFlags.None, baseInstance.InstanceScope);
            }

            m_Classes[name].AddClassData(scope);

            BSClassInstance table = new BSClassInstance(SourcePosition.Unknown, name, baseInstance, scope);
            
            return table;
        }

        public static BSClassInstance CreateInstance( string name, BSEngine engine )
        {
            BSScope classScope = new BSScope(engine);
            return CreateBaseInstance(name, classScope);
        }

        internal static void AddClass( BSClassExpression expr ) => m_Classes.Add( expr.Name, expr );

    }

}