﻿using System.Xml;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Xml
{

    public class XmlInterface:ABSScriptInterface
    {
        public XmlInterface( ) : base( "xml" )
        {
        }

        public override void AddApi( ABSTable root )
        {
            BSFunction toXml = new BSFunction("function createDoc(str)", FromXml,1 );
            root.InsertElement( new BSObject( "createDoc" ), toXml );
        }

        private ABSObject FromXml( ABSObject[] arg )
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(arg[0].ConvertString());

            return new XmlDocumentObject( doc );
        }
    }

}