using System.Reflection;
using System.Runtime.InteropServices;

namespace BadScript.Utils.Optimization.Compilation
{

    [StructLayout(LayoutKind.Sequential)]
    public struct BSSerializedHeader
    {
        public bool IsValidHeader => Magic[0] == 69 &&
                                     Magic[0] == 42 &&
                                     Magic[0] == 69 &&
                                     Magic[0] == 42;

        public byte[] Magic;
        public BSSerializerHints SerializerHints;
        public string BSSerializerFormatVersion;
        public static BSSerializedHeader CreateEmpty(BSSerializerHints hints)
        {
            return new BSSerializedHeader
            {
                Magic = new byte[] { 69, 42, 69, 42 },
                BSSerializerFormatVersion = $"v{Assembly.GetExecutingAssembly().GetName().Version}",
                SerializerHints = hints
            };
        }
    }

}