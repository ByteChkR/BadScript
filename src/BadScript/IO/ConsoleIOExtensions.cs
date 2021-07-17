using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace BadScript.IO
{

    public static class ConsoleIOExtensions
    {
        #region Public

        public static string GetFullName( this IConsoleIOEntry e )
        {
            StringBuilder sb = new StringBuilder();
            IConsoleIOEntry current = e.GetParent();
            sb.Append( e.GetName() );

            while ( current != null )
            {
                sb.Insert( 0, current.GetName() + "/" );
                current = current.GetParent();
            }

            sb.Insert( 0, e.GetRoot().GetRootPath() );

            return sb.ToString();
        }

        public static T ParseXML < T >( this IConsoleIOFile file )
        {
            using Stream s = file.Open();

            using TextReader tr = new StreamReader( s );

            XmlSerializer xs = new XmlSerializer( typeof( T ) );

            return ( T ) xs.Deserialize( tr );

        }

        public static void WriteXML < T >( this IConsoleIOFile file, T data )
        {
            using Stream s = file.Open();

            s.Position = 0;
            s.SetLength( 0 );
            using TextWriter tr = new StreamWriter( s );

            XmlSerializer xs = new XmlSerializer( typeof( T ) );
            xs.Serialize( tr, data );

        }

        #endregion
    }

}
