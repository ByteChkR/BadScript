using System;
using System.Reflection;
using BadScript.Common.Exceptions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Utils.Reflection
{

    public class BSReflectionScriptInterface : ABSScriptInterface
    {
        #region Public

        public BSReflectionScriptInterface() : base( "reflection" )
        {
        }

        public override void AddApi( ABSTable root )
        {
            root.InsertElement( new BSObject( "getType" ), new BSFunction( "function getType(fullName)", GetType, 1 ) );

            root.InsertElement(
                new BSObject( "loadAssembly" ),
                new BSFunction( "function loadAssembly(fullName)", LoadAssembly, 1, int.MaxValue ) );

            root.InsertElement(
                new BSObject( "wrapInstance" ),
                new BSFunction( "function wrapInstance(instance)", WrapInstance, 1 ) );

            root.InsertElement(
                new BSObject( "getConstructorData" ),
                new BSFunction( "function getConstructorData()", objects => TypeBuilder.GetConstructorData(), 0 ) );
        }

        #endregion

        #region Private

        private ABSObject GetType( ABSObject[] arg )
        {
            string s = arg[0].ConvertString();
            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();

            foreach ( Assembly asm in asms )
            {
                string fn = asm.FullName;

                if ( asm.IsDynamic )
                {
                    continue;
                }

                Type[] ts = asm.GetTypes();

                foreach ( Type type in ts )
                {
                    if ( type.Name == s || type.FullName == s )
                    {
                        return TypeBuilder.Build( type );
                    }
                }
            }

            throw new BSRuntimeException( "Could not find Type: " + s );
        }

        private ABSObject LoadAssembly( ABSObject[] arg )
        {
            foreach ( ABSObject absObject in arg )
            {
                Assembly asm = Assembly.Load( absObject.ConvertString() );

                if ( asm == null )
                {
                    throw new BSRuntimeException( "Could not Find Assembly: " + absObject.ConvertString() );
                }
                else
                {
                    Console.WriteLine( "Loaded Assembly: " + asm.FullName );
                }
            }

            return new BSObject( null );
        }

        private ABSObject WrapInstance( ABSObject[] arg )
        {
            object o = ( ( BSObject ) arg[0] ).GetInternalObject();

            return TypeBuilder.Build( o.GetType(), o );
        }

        #endregion
    }

}
