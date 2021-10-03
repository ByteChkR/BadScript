namespace BadScript.Parser.Expressions.Implementations.Block
{

    public class BSFunctionParameter
    {

        public string Name;
        public bool IsOptional;
        public bool NotNull;
        public bool IsArgArray;

        #region Public

        public BSFunctionParameter( string name, bool notNull, bool optional, bool isArgArray )
        {
            Name = name;
            NotNull = notNull;
            IsOptional = optional;
            IsArgArray = isArgArray;
        }

        #endregion

    }

}
