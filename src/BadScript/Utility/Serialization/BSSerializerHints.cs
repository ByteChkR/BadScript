﻿using System;

namespace BadScript.Utility.Serialization
{

    [Flags]
    public enum BSSerializerHints : byte
    {

        Default = 0,
        NoStringCache = 1

    }

}
