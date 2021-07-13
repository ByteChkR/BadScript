namespace BadScript.IO
{

    public interface IConsoleIORoot
    {
        IConsoleIOEntry GetChildAt( int index );

        int GetChildCount();

        string GetRootPath();
    }

}
