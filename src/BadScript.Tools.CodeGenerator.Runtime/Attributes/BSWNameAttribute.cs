using System;

namespace BadScript.Tools.CodeGenerator.Runtime.Attributes
{

    [AttributeUsage(
                       AttributeTargets.Method |
                       AttributeTargets.Field |
                       AttributeTargets.Property
                   )]
    public class BSWNameAttribute : Attribute
    {

        public readonly string Name;

        #region Public

        public BSWNameAttribute( string name )
        {
            Name = name;
        }

        #endregion

    }

}
