using System;

namespace BadScript.Utils.Optimization.Compilation
{

    [Flags]
    public enum BSSerializerHints:byte
    {
        Default,
        Compressed = 1
    }

}