using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BadScript.Exceptions;
using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Block.ForEach;
using BadScript.Scopes;
using BadScript.Types.References;

namespace BadScript.Types.Implementations.Types
{

    public sealed class BSClassInstance : ABSObject, IEnumerable < IForEachIteration >
    {
        public readonly string Name;
        private readonly BSScope m_InstanceScope;

        private readonly BSClassInstance m_BaseInstance;

        public BSScope InstanceScope => m_InstanceScope;

        public override bool IsNull() => false;

        #region Public

        public BSClassInstance(
            SourcePosition pos,
            string name,
            BSClassInstance baseInstance,
            BSScope instanceScope ) : base( pos )
        {
            m_BaseInstance = baseInstance;
            m_InstanceScope = instanceScope;

            if ( baseInstance != null )
            {
                m_InstanceScope.AddLocalVar( "base", baseInstance );
            }

            m_InstanceScope.AddLocalVar( "this", this );

            m_InstanceScope.AddLocalVar(
                                        "IsInstanceOf",
                                        new BSFunction(
                                                       "function IsInstanceOf(TypeName/TypeInstance)",
                                                       IsInstanceOf,
                                                       1
                                                      )
                                       );

            Name = name;
        }
        protected override int GetHashCodeImpl()
        {
            return Name.GetHashCode() ^ m_InstanceScope.GetHashCode();
        }

        public override bool Equals( ABSObject other )
        {
            return ReferenceEquals( this, other );
        }

        public IEnumerator < IForEachIteration > GetEnumerator()
        {
            return m_InstanceScope.GetEnumerator();
        }

        public override ABSReference GetProperty( string propertyName )
        {
            if ( m_InstanceScope.HasLocal( propertyName ) )
            {
                if ( propertyName == "this" || propertyName == "base" )
                {
                    return m_InstanceScope.Get( propertyName, true );
                }

                return m_InstanceScope.Get( propertyName );
            }

            throw new BSRuntimeException( $"Property {propertyName} does not exist in Type '{Name}'" );
        }

        public override bool HasProperty( string propertyName )
        {
            return m_InstanceScope.HasLocal( propertyName );
        }

        public override ABSObject Invoke( ABSObject[] args )
        {
            throw new BSRuntimeException( Position, $"Can not invoke '{this}'" );
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
        {
            if ( doneList.ContainsKey( this ) )
            {
                return "<recursion>";
            }

            doneList[this] = "{}";

            StringWriter sw = new StringWriter();
            IndentedTextWriter tw = new IndentedTextWriter( sw );
            tw.WriteLine( $"class {Name}" );
            tw.WriteLine( '{' );

            foreach ( IForEachIteration bsRuntimeIt in m_InstanceScope )
            {
                ABSObject[] objs = bsRuntimeIt.GetObjects();

                KeyValuePair < ABSObject, ABSObject > bsRuntimeObject =
                    new KeyValuePair < ABSObject, ABSObject >( objs[0], objs[1] );

                List < string > keyLines = bsRuntimeObject.Key.SafeToString( doneList ).
                                                           Split(
                                                                 new[] { '\n' },
                                                                 StringSplitOptions.RemoveEmptyEntries
                                                                ).
                                                           Where( x => !string.IsNullOrEmpty( x ) ).
                                                           ToList();

                List < string > valueLines = bsRuntimeObject.Value.SafeToString( doneList ).
                                                             Split(
                                                                   new[] { '\n' },
                                                                   StringSplitOptions.RemoveEmptyEntries
                                                                  ).
                                                             Where( x => !string.IsNullOrEmpty( x ) ).
                                                             ToList();

                tw.Indent = 1;

                for ( int i = 0; i < keyLines.Count; i++ )
                {
                    string keyLine = keyLines[i];

                    if ( i < keyLines.Count - 1 )
                    {
                        tw.WriteLine( keyLine );
                    }
                    else
                    {
                        tw.Write( keyLine + " = " );
                    }
                }

                tw.Indent = 2;

                for ( int i = 0; i < valueLines.Count; i++ )
                {
                    string valueLine = valueLines[i];
                    tw.WriteLine( valueLine );
                }
            }

            tw.Indent = 0;
            tw.WriteLine( '}' );

            doneList[this] = sw.ToString();

            return doneList[this];
        }

        public override void SetProperty( string propertyName, ABSObject obj )
        {
            m_InstanceScope.Set( propertyName, obj );
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

        #region Private

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private ABSObject IsInstanceOf( ABSObject[] arg )
        {
            if ( !arg[0].TryConvertString( out string name ) )
            {
                name = arg[0].GetProperty( "GetType" ).Invoke( Array.Empty < ABSObject >() ).ConvertString();
            }

            BSClassInstance instance = this;

            while ( instance != null )
            {
                if ( instance.Name == name )
                {
                    return BSObject.True;
                }

                instance = instance.m_BaseInstance;
            }

            return BSObject.False;
        }

        #endregion

    }

}
