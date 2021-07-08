namespace BadScript.IO
{

    public interface IConsoleIODirectory : IConsoleIOEntry
    {
        int GetChildCount();

        IConsoleIOEntry GetChildAt( int index );
    }

}