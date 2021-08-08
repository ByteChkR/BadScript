namespace BadScript.Tools.CodeGenerator.Runtime
{

    public interface IWrapperConstructorDataBase
    {
        bool HasType < T >();


        BSWrapperObject <T> Get<T>( object[] args);
    }

}