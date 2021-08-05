using System.IO;
using BadScript.IO;
using Newtonsoft.Json;

namespace BadScript.Console.Core.IO
{

    public static class ConfigExtensions
    {
        #region Public

        public static void EnsureExistParent( this IConsoleIOEntry e )
        {
            IConsoleIODirectory parent = e.GetParent();

            if ( parent == null )
            {
                return;
            }

            Directory.CreateDirectory( parent.GetFullName() );
        }

        public static void EnsureExistsSelf( this IConsoleIODirectory d )
        {
            d.EnsureExistParent();
            Directory.CreateDirectory( d.GetFullName() );
        }

        public static T ParseJson < T >( this IConsoleIOFile file )
        {
            using Stream s = file.Open();

            using TextReader tr = new StreamReader( s );

            return JsonConvert.DeserializeObject < T >( tr.ReadToEnd() );

        }

        public static void WriteJson( this IConsoleIOFile file, object data )
        {
            using Stream s = file.Open();

            s.Position = 0;
            s.SetLength( 0 );
            using TextWriter tr = new StreamWriter( s );

            tr.Write( JsonConvert.SerializeObject( data, Formatting.Indented ) );

        }

        #endregion
    }

}
