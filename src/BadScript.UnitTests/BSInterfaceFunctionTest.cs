namespace BadScript.NUnit.Utils
{

    public struct BSInterfaceFunctionTest
    {

        public string Name;
        public bool CrashIsPass;
        public string[] Arguments;
        public string ReturnObjectAction;

        public BSRunnableTestCase MakeTestCase(string name)
        {
            string generateSig = "";

            for ( int i = 0; i < Arguments.Length; i++ )
            {
                if ( i != 0 )
                    generateSig += ", ";

                generateSig += Arguments[i];
            }
            return new BSRunnableTestCase
                   {
                       Key = name+$".{Name}",
                       Source = $"return {name}({generateSig})",
                       ReturnObjectAction = ReturnObjectAction,
                       CrashIsPass = CrashIsPass
            };
        }

    }

}