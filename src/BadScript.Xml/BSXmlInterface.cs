using System.Xml;

using BadScript.Interfaces;
using BadScript.Types;
using BadScript.Types.Implementations;

namespace BadScript.Xml
{

    public class BSXmlInterface : ABSScriptInterface
    {

        #region Public

        public BSXmlInterface() : base( "Xml" )
        {
        }

        public override void AddApi( ABSTable root )
        {
            BSFunction toXml = new BSFunction( "function CreateDoc(str)", FromXml, 1 );
            root.InsertElement( new BSObject("CreateDoc"), toXml );
        }

        #endregion

        #region Private

        private ABSObject FromXml( ABSObject[] arg )
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml( arg[0].ConvertString() );

            return new XmlDocumentObject( doc );
        }

        #endregion

    }

}
