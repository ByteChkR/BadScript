using System;

using BadScript.Common.Types;

namespace BadScript.Tools.CodeGenerator.Runtime
{

    public interface IWrapperConstructorDataBase
    {

        ABSObject Get( Type t, object[] args );

        bool HasType( Type t );

        Type[] Types { get; }

    }

}
