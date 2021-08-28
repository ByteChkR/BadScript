namespace BadScript.Utils.Serialization
{

    public enum BSCompiledExpressionCode : byte
    {
        Unknown,
        ValueNull,
        ValueDecimal,
        ValueString,
        ReturnExpr,
        ContinueExpr,
        BreakExpr,
        TableExpr,
        ArrayExpr,
        ArrayAccessExpr,
        PropertyAccessExpr,
        InvocationExpr,
        NullCheckPropertyAccessExpr,
        AssignExpr,
        WhileExpr,
        TryExpr,
        IfExpr,
        FunctionDefinitionExpr,
        FunctionEnumerableDefinitionExpr,
        ForEachExpr,
        ForExpr,
        BinaryExpr,
        UnaryExpr,
    }

}
