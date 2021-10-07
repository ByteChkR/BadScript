using BadScript.Interfaces.Collection;
using BadScript.NUnit.Utils;

namespace BadScript.Tests.Interfaces.Collection
{

    public class CollectionInterfaceTestWrapper : ABSScriptInterfaceUnitTestWrapper
    {

        #region Public

        public CollectionInterfaceTestWrapper() : base( new BSCollectionInterface() )
        {
        }

        #endregion

    }

}
