namespace BadScript.IO
{

    public interface IConsoleIOEntry
    {
        bool Exists { get; }
        string GetName();

        IConsoleIORoot GetRoot();
        IConsoleIODirectory GetParent();

    }

}