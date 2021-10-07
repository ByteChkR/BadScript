using BadScript.Interfaces;
using BadScript.Interfaces.Versioning;
using BadScript.NUnit.Utils;

namespace BadScript.Tests.Interfaces.Versioning
{

    public class VersioningInterfaceTestWrapper : ABSScriptInterfaceUnitTestWrapper
    {

        public VersioningInterfaceTestWrapper() : base( new BSVersioningInterface() )
        {
        }

    }

    public class VersioningInterfaceTests : ABSInterfaceUnitTest <VersioningInterfaceTestWrapper>
    {

    }

}
