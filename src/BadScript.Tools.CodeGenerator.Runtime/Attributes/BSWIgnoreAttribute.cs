using System;

namespace BadScript.Tools.CodeGenerator.Runtime.Attributes
{

    [AttributeUsage(
        AttributeTargets.Method | 
        AttributeTargets.Class | 
        AttributeTargets.Struct |
        AttributeTargets.Field | 
        AttributeTargets.Property)]
    public class BSWIgnoreAttribute : Attribute { }

}