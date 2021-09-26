using System;

namespace BadScript.Utility.Serialization
{

    [Flags]
    public enum BSSerializerHints : byte
    {

        Default,
        Compressed = 1

    }

}
