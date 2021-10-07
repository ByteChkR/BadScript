using BadScript.Interfaces.Environment;
using BadScript.NUnit.Utils;

namespace BadScript.Tests.Interfaces.Environment
{

    public class EnvironmentInterfaceTestWrapper : ABSScriptInterfaceUnitTestWrapper
    {

        #region Public

        public EnvironmentInterfaceTestWrapper() : base(
                                                        new BSEnvironmentInterface(
                                                             BSEngineSettings.MakeDefault().Build()
                                                            )
                                                       )
        {
        }

        #endregion

    }

}
