using BadScript.Common.Types;

namespace BadScript.Common.Exceptions
{

    public class BSInvalidOperationException : BSRuntimeException
    {
        private static string GenerateOperationErrorText(string op, ABSObject[] o)
        {
            string r = $"Can not apply '{op}' between objects: ";

            for (int i = 0; i < o.Length; i++)
            {
                ABSObject absObject = o[i];

                if (i != o.Length - 1)
                    r += r + ", ";

                r += absObject;
            }

            return r;
        }
        public BSInvalidOperationException(string op, params ABSObject[] o) : base(GenerateOperationErrorText(op, o)) { }
    }

}
