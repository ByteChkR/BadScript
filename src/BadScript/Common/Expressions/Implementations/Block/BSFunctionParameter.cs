namespace BadScript.Common.Expressions.Implementations.Block
{

    public class BSFunctionParameter
    {
        public string Name;
        public bool IsOptional;
        public bool NotNull;

        public BSFunctionParameter( string name, bool notNull, bool optional )
        {
            Name = name;
            NotNull = notNull;
            IsOptional = optional;
        }
    }

}