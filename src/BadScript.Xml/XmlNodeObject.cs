using System.Collections.Generic;
using System.Xml;
using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;
using BadScript.Utils.Reflection;

namespace BadScript.Xml
{

    public class XmlNodeObject : ABSObject
    {
        protected XmlNode m_Node;
        protected Dictionary < string, ABSReference > m_Properties = new Dictionary < string, ABSReference >();

        public override bool IsNull => false;

        #region Public

        public XmlNodeObject( XmlNode node ) : base( SourcePosition.Unknown )
        {
            m_Node = node;

            m_Properties.Add(
                "parentNode",
                new BSReflectionReference( () => new XmlNodeObject( m_Node.ParentNode ), null ) );

            m_Properties.Add(
                "firstChild",
                new BSReflectionReference( () => new XmlNodeObject( m_Node.FirstChild ), null ) );

            m_Properties.Add(
                "findChild",
                new BSFunctionReference( new BSFunction("function findChild(name)", FindChildByName, 1 ) ) );

            m_Properties.Add(
                "lastChild",
                new BSReflectionReference( () => new XmlNodeObject( m_Node.LastChild ), null ) );

            m_Properties.Add(
                "nextSibling",
                new BSReflectionReference( () => new XmlNodeObject( m_Node.NextSibling ), null ) );

            m_Properties.Add(
                "previousSibling",
                new BSReflectionReference( () => new XmlNodeObject( m_Node.PreviousSibling ), null ) );

            m_Properties.Add(
                "childCount",
                new BSReflectionReference( () => new BSObject( ( decimal ) m_Node.ChildNodes.Count ), null ) );

            m_Properties.Add(
                "childAt",
                new BSFunctionReference( new BSFunction( "function childAt(index)", ChildAt, 1 ) ) );

            m_Properties.Add(
                "value",
                new BSReflectionReference(
                    () => new BSObject( m_Node.Value ),
                    o => m_Node.Value = o.ConvertString() ) );

            m_Properties.Add(
                "innerText",
                new BSReflectionReference(
                    () => new BSObject( m_Node.InnerText ),
                    o => m_Node.InnerText = o.ConvertString() ) );

            m_Properties.Add( "name", new BSReflectionReference( () => new BSObject( m_Node.Name ), null ) );

            m_Properties.Add(
                "hasChildNodes",
                new BSReflectionReference( () => m_Node.HasChildNodes ? BSObject.One : BSObject.Zero, null ) );

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

        #region Private

        private ABSObject ChildAt( ABSObject[] arg )
        {
            return new XmlNodeObject( m_Node.ChildNodes[( int ) arg[0].ConvertDecimal()] );
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
