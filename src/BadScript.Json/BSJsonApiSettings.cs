using System;

namespace BadScript.Json
{

    [Flags]
    public enum BSJsonApiSettings
    {
        None = 0,
        BS2Json = 1,
        Json2BS = 2,
        Full = BS2Json | Json2BS
    }

}
