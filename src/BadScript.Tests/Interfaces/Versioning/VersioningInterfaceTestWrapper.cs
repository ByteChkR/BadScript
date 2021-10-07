using BadScript.Interfaces.Versioning;
using BadScript.NUnit.Utils;

namespace BadScript.Tests.Interfaces.Versioning
{

    public class VersioningInterfaceTestWrapper : ABSScriptInterfaceUnitTestWrapper
    {

        #region Public

        public VersioningInterfaceTestWrapper() : base( new BSVersioningInterface() )
        {
        }

        #endregion

    }

}
