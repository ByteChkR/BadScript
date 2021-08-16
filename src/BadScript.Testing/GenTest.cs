using System;
using BadScript.Tools.CodeGenerator;

namespace BadScript.Testing
{

    public class GenTest
    {
        public static int Number
        {
            get;
            set;
        }
        public static void Main()
        {
            string src = WrapperGenerator.Generate < GenTest >();
        }
    }

}
