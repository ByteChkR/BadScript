using System;

namespace BadScript.Utils.Serialization
{

    [Flags]
    public enum BSSerializerHints : byte
    {
        Default,
        Compressed = 1
    }

}
