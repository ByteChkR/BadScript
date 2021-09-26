using System.Collections.Generic;

using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Value;
using BadScript.Common.Runtime;
using BadScript.Common.Types.References;

namespace BadScript.Common.Types.Implementations
{

    public static class BSClassBase
    {

        private static readonly Dictionary < string, BSClassExpression > m_Classes =
            new Dictionary < string, BSClassExpression >();

        #region Public

        public static void Clear()
        {
            m_Classes.Clear();
        }

        public static BSClassInstance CreateInstance( string name, BSEngine engine, ABSObject[] args )
        {
            BSScope classScope = new BSScope( engine );

            return CreateBaseInstance( name, classScope, args );
        }

        internal static void AddClass( BSClassExpression expr )
        {
            if ( m_Classes.ContainsKey( expr.Name ) )
            {
                throw new BSRuntimeException(
                                             $"Can not Create Class Definition because a Type with the name '{expr.Name}' does already exist."
                                            );
            }

            m_Classes.Add( expr.Name, expr );
        }

        #endregion

        #region Private

        private static BSClassInstance CreateBaseInstance( string name, BSScope scope, ABSObject[] args )
        {
            BSClassExpression expr = m_Classes[name];
            BSClassInstance baseInstance = null;

            if ( expr.BaseName != null )
            {
                baseInstance = CreateBaseInstance( expr.BaseName, scope, null );
                scope = new BSScope( BSScopeFlags.None, baseInstance.InstanceScope );
            }

            m_Classes[name].AddClassData( scope );

            BSClassInstance table = new BSClassInstance( SourcePosition.Unknown, name, baseInstance, scope );

            if ( args != null )
            {
                if ( table.HasProperty( table.Name ) )
                {
                    ABSObject func = table.GetProperty( table.Name ).ResolveReference();
                    func.Invoke( args );
                }
            }

            return table;
        }

        #endregion

    }

}
