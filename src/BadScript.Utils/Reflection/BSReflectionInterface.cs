using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using BadScript.Common.Exceptions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Interfaces;

namespace BadScript.Utils.Reflection
{

    public class BSReflectionInterface : ABSScriptInterface
    {

        private static BSReflectionInterface s_Instance;

        private static readonly Dictionary < Type, BSReflectedType > m_Types = new Dictionary < Type, BSReflectedType >();

        public static BSReflectionInterface Instance => s_Instance ?? ( s_Instance = new BSReflectionInterface() );


        public bool IsRecursionSafe( ABSObject o )
        {
            return o is IBSWrappedObject wo && IsRecursionSafe( wo.GetInternalObject() );
        }
        public bool IsRecursionSafe( object o )
        {
            return o == null || o is bool || o is string || BSReflectedObject.IsNumericType( o );
        }

        public ABSObject Wrap(object o )
        {
            if (o == null)
                return BSObject.Null;

            if (o is true)
                return BSObject.True;

            if (o is false)
                return BSObject.False;

            if (o is string)
                return new BSObject(o);

            if ( BSReflectedObject.IsNumericType( o ) )
            {
                return new BSObject( Convert.ChangeType( o, TypeCode.Decimal ) );
            }
            return m_Types[o.GetType()].Wrap( o );
        }

        public void AddType < T >() => AddType( typeof( T ) );
        public void AddType( Type t )
        {
            if ( m_Types.ContainsKey( t ) )
                return;

            m_Types[t] = new BSReflectedType( t );
        }

        public BSReflectedType Find( string name ) => m_Types.First( x => x.Key.Name == name ).Value;

        public ABSObject GetType < T >() => GetType( typeof( T ) );
        public ABSObject GetType( Type t )
        {
            if ( m_Types.ContainsKey( t ) )
                return m_Types[t];

            return m_Types[t] = new BSReflectedType( t );
        }

        private BSReflectionInterface(  ) : base( "reflection" )
        {
        }

        public override void AddApi( ABSTable root )
        {
            root.InsertElement( new BSObject( "create" ), new BSFunction(
                                                                         "function create(typename, args0, args...)",
                                                                         a=>Find(a[0].ConvertString()).Create(a.Skip(1).ToArray()),
                                                                         1,
                                                                         int.MaxValue
                                                                        ));

            root.InsertElement(
                               new BSObject( "getType" ),
                               new BSFunction( "function getType(typename)", a => Find(a[0].ConvertString()),1)
                              );
        }

        private ABSObject LoadAssembly(ABSObject[] arg)
        {
            foreach (ABSObject absObject in arg)
            {
                Assembly asm = Assembly.Load(absObject.ConvertString());

                if (asm == null)
                {
                    throw new BSRuntimeException("Could not Find Assembly: " + absObject.ConvertString());
                }
                else
                {
                    Console.WriteLine("Loaded Assembly: " + asm.FullName);
                }
            }

            return BSObject.Null;
        }
    }

}