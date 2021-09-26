using System.IO;

namespace BadScript.IO
{

    public class Crc
    {

        private static readonly Crc s_Instance = new Crc();

        private long[] m_PTable = new long[256];

        private long m_Poly = 0xEDB88320;

        #region Public

        public Crc()
        {
            int i;

            for ( i = 0; i < 256; i++ )
            {
                long crc = i;

                int j;

                for ( j = 0; j < 8; j++ )
                {
                    if ( ( crc & 0x1 ) == 1 )
                    {
                        crc = ( crc >> 1 ) ^ m_Poly;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }

                m_PTable[i] = crc;
            }
        }

        public static uint GetCrc( string fileName )
        {
            return s_Instance.GetCrc32( fileName );
        }

        public uint GetCrc32( string fileName )
        {
            //4KB Buffer
            int bufferSize = 0x1000;

            FileStream fs = new FileStream( fileName, FileMode.Open, FileAccess.Read );
            long streamLength = fs.Length;

            long crc = 0xFFFFFFFF;

            while ( streamLength > 0 )
            {
                if ( streamLength < bufferSize )
                {
                    bufferSize = ( int )streamLength;
                }

                byte[] buffer = new byte[bufferSize];

                fs.Read( buffer, 0, bufferSize );

                for ( int i = 0; i < bufferSize; i++ )
                {
                    crc = ( ( ( crc & 0xFFFFFF00 ) / 0x100 ) & 0xFFFFFF ) ^ m_PTable[buffer[i] ^ ( crc & 0xFF )];
                }

                streamLength -= bufferSize;
            }

            fs.Close();
            crc = -crc - 1; // !(CRC)

            return ( uint )crc;
        }

        #endregion

    }

}
