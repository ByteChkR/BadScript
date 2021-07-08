using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.StringUtils
{

    public static class StringUtilsApiExtensions
    {

        public static void InsertElement(this ABSTable t, string name, ABSObject o)
        {
            t.InsertElement(new BSObject(name), o);
        }

    }

}