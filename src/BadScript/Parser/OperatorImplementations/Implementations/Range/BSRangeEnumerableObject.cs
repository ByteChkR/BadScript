using System;
using System.Collections;
using System.Collections.Generic;

using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Block.ForEach;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Parser.OperatorImplementations.Implementations.Range
{

    public class BSRangeEnumerableObject : ABSObject, IEnumerable < IForEachIteration >
    {

        private readonly int m_Start;
        private readonly int m_End;
        private readonly int m_Step;

        #region Public

        public BSRangeEnumerableObject( SourcePosition pos, int start, int end ) : base( pos )
        {
            m_Start = start;
            m_End = end;
            m_Step = m_End < m_Start ? -1 : 1;
        }

        public override bool Equals( ABSObject other )
        {
            return other is BSRangeEnumerableObject o && ReferenceEquals( this, o );
        }

        public IEnumerator < IForEachIteration > GetEnumerator()
        {
            for ( int i = m_Start; i <= m_End; i += m_Step )
            {
                yield return new ForEachIteration( new BSObject( ( decimal )i ) );
            }
        }

        public override ABSReference GetProperty( string propertyName )
        {
            throw new NotSupportedException();
        }

        public override bool HasProperty( string propertyName )
        {
            return false;
        }

        public override ABSObject Invoke( ABSObject[] args )
        {
            throw new NotSupportedException();
        }

        public override bool IsNull()
        {
            return false;
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
        {
            return $"Range Enumerator({m_Start}..{m_End})";
        }

        public override void SetProperty( string propertyName, ABSObject obj )
        {
            throw new NotSupportedException();
        }

        public override bool TryConvertBool( out bool v )
        {
            v = false;

            return false;
        }

        public override bool TryConvertDecimal( out decimal d )
        {
            d = 0;

            return false;
        }

        public override bool TryConvertString( out string v )
        {
            v = null;

            return false;
        }

        #endregion

        #region Protected

        protected override int GetHashCodeImpl()
        {
            return m_Start.GetHashCode() ^ m_End.GetHashCode();
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
