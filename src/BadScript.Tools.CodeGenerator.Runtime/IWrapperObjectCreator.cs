namespace BadScript.Tools.CodeGenerator.Runtime
{

    public interface IWrapperObjectCreator
    {
        int ArgCount { get; }
        object Create( object[] args );
    }

}