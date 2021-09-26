using System.Collections;
using System.Collections.Generic;

using BadScript.Common.Exceptions;
using BadScript.Common.Expressions.Implementations.Block.ForEach;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.References;

namespace BadScript.Common.Expressions.Implementations.Block
{

    public class BSFunctionEnumeratorObject : ABSObject, IEnumerable < IForEachIteration >
    {

        private int m_State;
        private readonly BSExpression[] m_Block;
        private readonly (string, ABSObject)[] m_Arguments;
        private readonly string m_DebugData;
        private readonly BSScope m_Scope;

        public override bool IsNull => false;

        #region Public

        public BSFunctionEnumeratorObject(
            SourcePosition pos,
            string debugData,
            BSExpression[] block,
            (string, ABSObject)[] args,
            BSScope scope ) : base( pos )
        {
            m_DebugData = debugData;
            m_Block = block;
            m_Arguments = args;
            m_Scope = scope;

            for ( int i = 0; i < m_Arguments.Length; i++ )
            {
                ( string name, ABSObject obj ) = m_Arguments[i];

                scope.AddLocalVar( name, obj );
            }
        }

        public override bool Equals( ABSObject other )
        {
            return ReferenceEquals( this, other );
        }

        public IEnumerator < IForEachIteration > GetEnumerator()
        {
            for ( ; m_State < m_Block.Length; m_State++ )
            {
                BSExpression buildScriptExpression = m_Block[m_State];
                buildScriptExpression.Execute( m_Scope );

                if ( m_Scope.BreakExecution )
                {
                    ABSObject r = m_Scope.Return;
                    m_Scope.SetFlag( BSScopeFlags.None );

                    yield return new ForEachIteration( new[] { r } );
                }
            }
        }

        public override ABSReference GetProperty( string propertyName )
        {
            throw new BSRuntimeException( Position, $"Property {propertyName} does not exist" );
        }

        public override bool HasProperty( string propertyName )
        {
            return false;
        }

        public override ABSObject Invoke( ABSObject[] args )
        {
            throw new BSRuntimeException( "Function Enumerators can only be enumerated" );
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
        {
            return m_DebugData;
        }

        public override void SetProperty( string propertyName, ABSObject obj )
        {
            throw new BSRuntimeException( Position, $"Property {propertyName} does not exist" );
        }

        public override bool TryConvertBool( out bool v )
        {
            v = true;

            return true;
        }

        public override bool TryConvertDecimal( out decimal d )
        {
            d = 1;

            return false;
        }

        public override bool TryConvertString( out string v )
        {
            v = m_DebugData;

            return true;
        }

        #endregion

        #region Private

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }

}
