﻿using System.Text;
using System.Xml;

using BadScript.Types;
using BadScript.Types.Implementations;

namespace BadScript.Xml
{

    public class XmlDocumentObject : XmlNodeObject
    {

        #region Public

        public XmlDocumentObject( XmlDocument node ) : base( node )
        {
            m_Properties.Add(
                             "toString",
                             new BSFunctionReference( new BSFunction( "function toString()", DocToString, 0 ) )
                            );
        }

        #endregion

        #region Private

        private static string Beautify( XmlDocument doc )
        {
            StringBuilder sb = new StringBuilder();

            XmlWriterSettings settings = new XmlWriterSettings
                                         {
                                             Indent = true,
                                             IndentChars = "  ",
                                             NewLineChars = "\r\n",
                                             NewLineHandling = NewLineHandling.Replace
                                         };

            using ( XmlWriter writer = XmlWriter.Create( sb, settings ) )
            {
                doc.Save( writer );
            }

            return sb.ToString();
        }

        private ABSObject DocToString( ABSObject[] arg )
        {
            XmlDocument xmlDoc = ( XmlDocument )m_Node;

            return new BSObject( Beautify( xmlDoc ) );
        }

        #endregion

    }

}
