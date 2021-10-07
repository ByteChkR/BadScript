using System.Collections.Generic;
using System.IO;

using BadScript.Interfaces;
using BadScript.Types;

using Newtonsoft.Json;

using NUnit.Framework;

namespace BadScript.NUnit.Utils
{

    public abstract class ABSScriptInterfaceUnitTestWrapper : ABSScriptInterface
    {

        protected string SuggestedTestDataPath
        {
            get
            {
                string d = Path.Combine( TestContext.CurrentContext.TestDirectory, "badscript-test-data" );
                Directory.CreateDirectory(d);

                return Path.Combine(d, Name + ".json");
            }
        }
        
        protected virtual BSInterfaceTestCaseData GetPersistentTestCaseData()
        {
            string f = SuggestedTestDataPath;
            if (File.Exists(SuggestedTestDataPath))
                return JsonConvert.DeserializeObject<BSInterfaceTestCaseData>(File.ReadAllText(f));

            return new BSInterfaceTestCaseData
                   {
                       FunctionTests = new List<BSInterfaceFunctionTestMatrix>(),
                       PropertyTests = new List<BSInterfacePropertyTest>()
                   };
        }

        protected virtual void SetPersistentTestCaseData(BSInterfaceTestCaseData data)
        {
            File.WriteAllText(SuggestedTestDataPath, JsonConvert.SerializeObject(data, Formatting.Indented));
        }

        public void TearDown()
        {
            SetPersistentTestCaseData( TestCases );
        }

        private readonly BSInterfaceTestCaseData m_GeneratedData;

        public BSInterfaceTestCaseData TestCases => GetPersistentTestCaseData().Merge( m_GeneratedData );

        private readonly ABSScriptInterface m_TestInterface;

        protected ABSScriptInterfaceUnitTestWrapper(ABSScriptInterface i ) : base( i.Name )
        {
            m_TestInterface = i;
            m_GeneratedData = i.GenerateTestCaseData();
        }

        public override void AddApi( ABSTable root )
        {
            m_TestInterface.AddApi( root );
        }

    }

}