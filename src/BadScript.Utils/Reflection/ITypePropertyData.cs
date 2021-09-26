using System;

using BadScript.Common.Types;

namespace BadScript.Utils.Reflection
{

    internal interface ITypePropertyData
    {

        Func < ABSObject > MakeGetter( object instance );

        Action < ABSObject > MakeSetter( object instance );

    }

}
