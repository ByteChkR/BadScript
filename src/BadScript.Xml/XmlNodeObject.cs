using System.Collections.Generic;
using System.Xml;

using BadScript.Exceptions;
using BadScript.Parser.Expressions;
using BadScript.Reflection;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;
using BadScript.Types.References.Implementations;

namespace BadScript.Xml
{

    public class XmlNodeObject : ABSObject
    {

        protected XmlNode m_Node;
        protected Dictionary < string, ABSReference > m_Properties = new Dictionary < string, ABSReference >();

        #region Public

        public XmlNodeObject( XmlNode node ) : base( SourcePosition.Unknown )
        {
            m_Node = node;

            m_Properties.Add(
                             "ParentNode",
                             new BSReflectionReference( () => new XmlNodeObject( m_Node.ParentNode ), null )
                            );

            m_Properties.Add(
                             "FirstChild",
                             new BSReflectionReference( () => new XmlNodeObject( m_Node.FirstChild ), null )
                            );

            m_Properties.Add(
                             "FindChild",
                             new BSFunctionReference( new BSFunction( "function FindChild(name)", FindChildByName, 1 ) )
                            );

            m_Properties.Add(
                             "LastChild",
                             new BSReflectionReference( () => new XmlNodeObject( m_Node.LastChild ), null )
                            );

            m_Properties.Add(
                             "NextSibling",
                             new BSReflectionReference( () => new XmlNodeObject( m_Node.NextSibling ), null )
                            );

            m_Properties.Add(
                             "PreviousSibling",
                             new BSReflectionReference( () => new XmlNodeObject( m_Node.PreviousSibling ), null )
                            );

            m_Properties.Add(
                             "ChildCount",
                             new BSReflectionReference( () => new BSObject( ( decimal )m_Node.ChildNodes.Count ), null )
                            );

            m_Properties.Add(
                             "ChildAt",
                             new BSFunctionReference( new BSFunction( "function ChildAt(index)", ChildAt, 1 ) )
                            );

            m_Properties.Add(
                             "Value",
                             new BSReflectionReference(
                                                       () => new BSObject( m_Node.Value ),
                                                       o => m_Node.Value = o.ConvertString()
                                                      )
                            );

            m_Properties.Add(
                             "InnerText",
                             new BSReflectionReference(
                                                       () => new BSObject( m_Node.InnerText ),
                                                       o => m_Node.InnerText = o.ConvertString()
                                                      )
                            );

            m_Properties.Add( "Name", new BSReflectionReference( () => new BSObject( m_Node.Name ), null ) );

            m_Properties.Add(
                             "HasChildNodes",
                             new BSReflectionReference(
                                                       () => m_Node.HasChildNodes ? BSObject.True : BSObject.False,
                                                       null
                                                      )
                            );
        }

        public override bool Equals( ABSObject other )
        {
            return ReferenceEquals( this, other );
        }

        public override ABSReference GetProperty( string propertyName )
        {
            return m_Properties[propertyName];
        }

        public override bool HasProperty( string propertyName )
        {
            return m_Properties.ContainsKey( propertyName );
        }

        public override ABSObject Invoke( ABSObject[] args )
        {
            throw new BSRuntimeException( "Can not Invoke XML Document Object" );
        }

        public override bool IsNull()
        {
            return false;
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
        {
            return m_Node.ToString();
        }

        public override void SetProperty( string propertyName, ABSObject obj )
        {
            throw new BSRuntimeException( "Can not edit properties on a XML Document Object" );
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
            v = SafeToString();

            return true;
        }

        #endregion

        #region Protected

        protected override int GetHashCodeImpl()
        {
            return m_Node.GetHashCode() ^ m_Properties.GetHashCode();
        }

        #endregion

        #region Private

        private ABSObject ChildAt( ABSObject[] arg )
        {
            return new XmlNodeObject( m_Node.ChildNodes[( int )arg[0].ConvertDecimal()] );
        }

        private ABSObject FindChildByName( ABSObject[] arg )
        {
            string name = arg[0].ConvertString();

            foreach ( XmlNode child in m_Node.ChildNodes )
            {
                if ( child.Name == name )
                {
                    return new XmlNodeObject( child );
                }
            }

            throw new BSRuntimeException( "Can not find Child: " + name );
        }

        #endregion

    }

}
