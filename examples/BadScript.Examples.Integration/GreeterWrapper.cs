using System;
using System.Collections.Generic;

using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;
using BadScript.Utility.Reflection;

namespace BadScript.Examples.Integration
{

    public class GreeterWrapper : ABSObject
    {

        private readonly Greeter m_Object;

        private readonly ABSReference m_Name;
        private readonly BSFunction m_GreetFunction;
        private readonly ABSReference m_GreetReference;

        public override bool IsNull => false;

        #region Public

        public GreeterWrapper( Greeter obj ) : base( SourcePosition.Unknown )
        {
            m_Object = obj; //Store the reference to be able to check for equality

            m_Name = new BSReflectionReference(
                                               () => new BSObject(
                                                                  m_Object.Name
                                                                 ), //Getter. Wrap the string into a BSObject
                                               value => m_Object.Name =
                                                            value.IsNull
                                                                ? null
                                                                : value.
                                                                    ConvertString() //Setter. Convert and Assign Value.
                                              );

            m_GreetFunction = new BSFunction(
                                             "function Greet()", //Debug info
                                             args =>
                                             {
                                                 m_Object.Greet(); //Display Greeting

                                                 return BSObject.Null;
                                             },
                                             0 //Argument Count
                                            );

            m_GreetReference = new BSFunctionReference( //Function Reference is readonly
                                                       m_GreetFunction
                                                      );
        }

        public override bool Equals( ABSObject other )
        {
            return other is GreeterWrapper ow && m_Object == ow.m_Object;
        }

        public override ABSReference GetProperty( string propertyName )
        {
            if ( propertyName == "Name" )
            {
                return m_Name;
            }

            if ( propertyName == "Greet" )
            {
                return m_GreetReference;
            }

            throw new BSRuntimeException( $"Can not find Property '{propertyName}'" );
        }

        public override bool HasProperty( string propertyName )
        {
            return propertyName == "Name" || propertyName == "Greet";
        }

        public override ABSObject Invoke( ABSObject[] args )
        {
            throw new BSRuntimeException( $"Can not find Invoke Object '{this}'" );
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
        {
            //Unused in this Example.
            return $"Greeting Wrapper Object: {m_Object}";
        }

        public override void SetProperty( string propertyName, ABSObject obj )
        {
            //Because we are using BSReflectionReference and BSFunctionReference we do not need this function.
            //  This function gets used when the wrapper uses BSTableReferences
            throw new NotSupportedException( "Can not set properties on this object" );
        }

        public override bool TryConvertBool( out bool v )
        {
            //We do not allow any kind of conversion to base types
            v = false;

            return false;
        }

        public override bool TryConvertDecimal( out decimal d )
        {
            //We do not allow any kind of conversion to base types
            d = 0;

            return false;
        }

        public override bool TryConvertString( out string v )
        {
            //We do not allow any kind of conversion to base types
            v = null;

            return false;
        }

        #endregion

    }

}
