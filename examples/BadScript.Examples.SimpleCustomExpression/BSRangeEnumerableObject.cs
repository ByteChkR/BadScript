using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using BadScript.ConsoleUtils;
using BadScript.Parser;
using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Block.ForEach;
using BadScript.Parser.OperatorImplementations;
using BadScript.Parser.Operators;
using BadScript.Parser.Operators.Implementations;
using BadScript.Scopes;
using BadScript.Serialization;
using BadScript.Serialization.Serializers;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Examples.SimpleCustomExpression
{

    public class BSRangeEnumerableObject : ABSObject, IEnumerable <IForEachIteration>
    {

        private readonly int m_Start;
        private readonly int m_End;
        public BSRangeEnumerableObject( SourcePosition pos,int start, int end) : base( pos )
        {
            m_Start =start;
            m_End = end;

        }

        public override bool IsNull => false;

        public override bool Equals( ABSObject other )
        {
            return other is BSRangeEnumerableObject o && ReferenceEquals(this, o);
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

        public IEnumerator < IForEachIteration > GetEnumerator()
        {
            for ( int i = m_Start; i < m_End; i++ )
            {
                yield return new ForEachIteration( new BSObject( ( decimal )i ) );
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

}
