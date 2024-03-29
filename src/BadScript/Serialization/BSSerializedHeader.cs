﻿using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace BadScript.Serialization
{

    [StructLayout( LayoutKind.Sequential )]
    public struct BSSerializedHeader
    {

        public bool IsValidHeader =>
            Magic[0] == 69 &&
            Magic[1] == 42 &&
            Magic[2] == 69 &&
            Magic[3] == 42;

        public byte[] Magic;
        public BSSerializerHints SerializerHints;
        public string BSSerializerFormatVersion;
        public List < string > StringTable;

        public static BSSerializedHeader CreateEmpty( BSSerializerHints hints )
        {
            return new BSSerializedHeader
                   {
                       Magic = new byte[] { 69, 42, 69, 42 },
                       BSSerializerFormatVersion =
                           $"v{Assembly.GetExecutingAssembly().GetName().Version}",
                       SerializerHints = hints,
                       StringTable = new List < string >()
                   };
        }

    }

}
