namespace BadScript.NUnit.Utils
{

    public struct BSRunnableTestCase
    {

        public string Key;
        public string Source;
        public string ReturnObjectAction;
        public bool CrashIsPass;

        public override string ToString()
        {
            return Key;
        }

    }

}