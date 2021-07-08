using System.IO;
using BadScript.IO;
using Newtonsoft.Json;

namespace BadScript.Console.IO
{

    public static class ConfigExtensions
    {
        public static void EnsureExistParent( this IConsoleIOEntry e )
        {
            IConsoleIODirectory parent = e.GetParent();

            if ( parent == null )
                return;
            Directory.CreateDirectory( parent.GetFullName() );
        }
        public static void WriteJson(this IConsoleIOFile file, object data)
        {
            using Stream s = file.Open();

            s.Position = 0;
            s.SetLength(0);
            using TextWriter tr = new StreamWriter(s);

            tr.Write(JsonConvert.SerializeObject(data));

        }
        public static T ParseJson < T >( this IConsoleIOFile file )
        {
            using Stream s = file.Open();

            using TextReader tr = new StreamReader( s );

            return JsonConvert.DeserializeObject < T >( tr.ReadToEnd() );

        }
    }

}
