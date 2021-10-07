namespace BadScript.UnitTests.DataObjects
{

    public struct BSInterfacePropertyTest
    {

        public string Name;
        public string ReturnObjectAction;

        public BSRunnableTestCase MakeTestCase()
        {
            return new BSRunnableTestCase
                   {
                       Key = Name,
                       Source = $"return {Name}",
                       ReturnObjectAction = ReturnObjectAction
                   };
        }

    }

}
