using System;
using System.IO;
using System.IO.Compression;

using Newtonsoft.Json;

namespace BadScript.Console.AppPackage
{

    public class BSAppPackage : IDisposable
    {

        private const string MANIFEST_PATH = "manifest.json";

        private readonly ZipArchive m_Archive;

        public BSAppPackageManifest Manifest { get; }

        #region Public

        public BSAppPackage( string file )
        {
            m_Archive = ZipFile.OpenRead( file );
            Manifest = ReadManifest();
        }

        public void Dispose()
        {
            m_Archive.Dispose();
        }

        public Stream GetItem( string path )
        {
            ZipArchiveEntry entry = m_Archive.GetEntry( path );

            if ( entry == null )
            {
                throw new BSAppPackageException( "Invalid App Package. No Executable Found!" );
            }

            return entry.Open();
        }

        public Stream GetResource( string path )
        {
            return GetItem( Path.Combine( Manifest.ResourcePath, path ) );
        }

        public string GetSource()
        {
            ZipArchiveEntry entry = m_Archive.GetEntry( Manifest.ExecutablePath );

            if ( entry == null )
            {
                throw new BSAppPackageException( "Invalid App Package. No Executable Found!" );
            }

            using TextReader tr = new StreamReader( entry.Open() );

            return tr.ReadToEnd();
        }

        #endregion

        #region Private

        private BSAppPackageManifest ReadManifest()
        {
            ZipArchiveEntry entry = m_Archive.GetEntry( MANIFEST_PATH );

            if ( entry == null )
            {
                throw new BSAppPackageException( "Invalid App Package. No Manifest Found!" );
            }

            using TextReader tr = new StreamReader( entry.Open() );

            return JsonConvert.DeserializeObject < BSAppPackageManifest >( tr.ReadToEnd() );
        }

        #endregion

    }

}
