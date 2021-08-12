using System;
using BadScript.Common.Types;

namespace BadScript.Tools.CodeGenerator.Runtime
{

    public interface IWrapperConstructorDataBase
    {
        Type[] Types { get; }

        bool HasType < T >();


        ABSObject Get(Type t, object[] args);

    }

}