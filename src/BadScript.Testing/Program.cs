using System.Collections.Generic;
using System.IO;
using BadScript.Common.Types;
using BadScript.ConsoleUtils;
using BadScript.Core;
using BadScript.Interfaces;
using BadScript.Settings;
using BadScript.Tools.CodeGenerator;
using BadScript.Tools.CodeGenerator.Runtime;

namespace BadScript.Testing
{

    internal class Program
    {
        #region Private

        private static string s_TestCode = @"
enumerable te(a, b)
{
    iterations = 1
    return iterations * (a + b)
    iterations += 1
    return iterations * (a + b)
}

foreach v in te(2, 5)
{
    console.print(v)
}
";
        private static void Main( string[] args )
        {
            BSSettings.BsRoot.LoadFromDirectory( Path.GetFullPath( "./bs-data/settings" ) );
            ConsoleApi cout = new ConsoleApi();
            BadScriptCoreApi core = new BadScriptCoreApi();
            BSEngine e = new BSEngine(
                BSParserSettings.Default,
                new Dictionary<string, ABSObject>(),
                new List<ABSScriptInterface> { cout, core });
            

            e.LoadInterface("console");
            e.LoadInterface("core");

            e.LoadString(false, s_TestCode, new ABSObject[0]);

            BSSettings.BsRoot.SaveToDirectory(Path.GetFullPath("./bs-data/settings"));
        }

        #endregion
    }

}
