using System;
using BadScript.Tools.CodeGenerator;

namespace BadScript.Testing
{
    public class Test
    {
        public float MyValue;
    }

    class Program
    {
        static void Main(string[] args)
        {
            string str = WrapperGenerator.Generate < Test >();
        }
    }
}
