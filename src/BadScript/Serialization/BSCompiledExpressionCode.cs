﻿namespace BadScript.Serialization
{

    public enum BSCompiledExpressionCode : byte
    {

        Unknown,
        ValueNull,
        ValueDecimal,
        ValueString,
        ValueFormattedString,
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
        ForEachExpr,
        ForExpr,
        BinaryExpr,
        UnaryExpr,
        CustomBlock,
        ClassDefExpr,
        NewClassExpr,
        NamespaceDefExpr,
        UsingDefExpr,
        CustomNamedExpression

    }

}
