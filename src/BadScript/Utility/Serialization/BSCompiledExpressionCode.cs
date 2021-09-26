namespace BadScript.Utility.Serialization
{

    public enum BSCompiledExpressionCode : byte
    {

        Unknown,
        ValueNull,
        ValueDecimal,
        ValueString,
        ValueBoolean,
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
        CustomBlock,
        ClassDefExpr,
        NewClassExpr,

    }

}
