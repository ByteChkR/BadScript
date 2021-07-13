namespace BadScript.IO
{

    public interface IConsoleIOEntry
    {
        bool Exists { get; }

        string GetName();

        IConsoleIODirectory GetParent();

        IConsoleIORoot GetRoot();
    }

}
