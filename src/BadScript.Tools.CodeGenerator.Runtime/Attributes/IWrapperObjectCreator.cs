namespace BadScript.Tools.CodeGenerator.Runtime.Attributes
{

    public interface IWrapperObjectCreator
    {
        int ArgCount { get; }
        object Create( object[] args );
    }

}