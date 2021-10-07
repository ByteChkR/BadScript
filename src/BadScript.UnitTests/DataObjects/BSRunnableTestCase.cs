namespace BadScript.UnitTests.DataObjects
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
