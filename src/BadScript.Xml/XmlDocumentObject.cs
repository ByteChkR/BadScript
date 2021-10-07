using System.Text;
using System.Xml;

using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References.Implementations;

namespace BadScript.Xml
{

    public class XmlDocumentObject : XmlNodeObject
    {

        #region Public

        public XmlDocumentObject( XmlDocument node ) : base( node )
        {
            m_Properties.Add(
                             "ToString",
                             new BSFunctionReference( new BSFunction( "function ToString()", DocToString, 0 ) )
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
