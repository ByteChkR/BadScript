using System;

namespace BadScript.Common.Runtime
{

    [Flags]
    public enum BSScopeFlags
    {

        None = 0,
        Break = 1,
        Continue = 2,
        Return = 4,
        Function = Return,
        IfBlock = Function,
        Loop = Return | Break | Continue

    }

}
