using System;

namespace BadScript.Serialization
{

    [Flags]
    public enum BSSerializerHints : byte
    {

        Default = 0,
        NoStringCache = 1

    }

}
