﻿using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Json
{
    public class BS2JsonInterface : ABSScriptInterface
    {
        public BS2JsonInterface() : base("json")
        {
        }

        public override void AddApi(ABSTable root)
        {
            BSFunction f = new BSFunction("function toJson(jsonStr)", BS2Json.Convert, 1);
            root.InsertElement(new BSObject("toJson"), f);
        }
    }

}
