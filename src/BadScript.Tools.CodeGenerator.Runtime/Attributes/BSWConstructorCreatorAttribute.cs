using System;
using System.Linq;

namespace BadScript.Tools.CodeGenerator.Runtime.Attributes
{

    [AttributeUsage(
        AttributeTargets.Class | 
        AttributeTargets.Struct)]
    public class BSWConstructorCreatorAttribute : Attribute
    {
        public Type ConstructorCreatorType;

        private bool HasValidConstructor =>
            ConstructorCreatorType.GetConstructors().Any( x => x.GetParameters().Length == 0 );

        private bool IsCreatorType => typeof( IWrapperObjectCreator ).IsAssignableFrom( ConstructorCreatorType );

        public BSWConstructorCreatorAttribute(Type creatorType)
        {
            ConstructorCreatorType = creatorType;

            if (!IsCreatorType)
            {
                throw new Exception(
                    $"Invalid Creator. The Creator must inherit from '{nameof(IWrapperObjectCreator)}'");
            }

            if (!HasValidConstructor)
            {
                throw new Exception(
                    $"Creator Type: '{ConstructorCreatorType.Name}' does not have an empty Constructor");
            }

        }
    }

}