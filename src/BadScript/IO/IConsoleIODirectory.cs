namespace BadScript.IO
{

    public interface IConsoleIODirectory : IConsoleIOEntry
    {
        IConsoleIOEntry GetChildAt( int index );

        int GetChildCount();
    }

}
