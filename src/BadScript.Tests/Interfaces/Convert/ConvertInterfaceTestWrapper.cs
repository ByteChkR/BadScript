using BadScript.Interfaces.Convert;
using BadScript.NUnit.Utils;

namespace BadScript.Tests.Interfaces.Convert
{

    public class ConvertInterfaceTestWrapper : ABSScriptInterfaceUnitTestWrapper
    {

        #region Public

        public ConvertInterfaceTestWrapper() : base( new BSConvertInterface() )
        {
        }

        #endregion

    }

}
