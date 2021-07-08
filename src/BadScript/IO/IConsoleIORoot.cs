namespace BadScript.IO
{

    public interface IConsoleIORoot
    {
        string GetRootPath();
        int GetChildCount();

        IConsoleIOEntry GetChildAt(int index);
    }

}