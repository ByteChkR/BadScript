using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using BadScript.Common.Expressions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Utils.Reflection
{

    public class BSReflectedMethod : BSFunction
    {

        private readonly MethodInfo[] m_MethodInfos;
        private readonly object m_Instance;

        #region Public

        public BSReflectedMethod( object instance, MethodInfo[] mis ) : base(
                                                                             SourcePosition.Unknown,
                                                                             $"function  {mis[0].Name}(...)",
                                                                             null,
                                                                             mis.Min( x => x.GetParameters().Length ),
                                                                             mis.Max( x => x.GetParameters().Length )
                                                                            )
        {
            m_Instance = instance;
            m_MethodInfos = mis;
            SetFunc( InvokeMethod );
        }

        public static MethodInfo FindMethod( MethodInfo[] mis, object[] args )
        {
            foreach ( MethodInfo methodInfo in mis )
            {
                ParameterInfo[] pis = methodInfo.GetParameters();
                int min = 0;
                int max = pis.Length;

                foreach ( ParameterInfo parameterInfo in pis )
                {
                    if ( parameterInfo.IsOptional )
                    {
                        break;
                    }
                    else
                    {
                        min++;
                    }
                }

                if ( min <= args.Length && max >= args.Length && CheckTypes( pis, args ) )
                {
                    return methodInfo;
                }
            }

            throw new Exception(); //TODO: Do better
        }

        public static object[] GetObjects( ABSObject[] args )
        {
            List < object > types = new List < object >();

            foreach ( ABSObject absObject in args )
            {
                if ( absObject is IBSWrappedObject wo )
                {
                    object o = wo.GetInternalObject();

                    if ( o == null )
                    {
                        throw new Exception(); //TODO: Better Exception
                    }

                    types.Add( o );
                }
            }

            return types.ToArray();
        }

        public override string ToString()
        {
            return DebugData;
        }

        #endregion

        #region Private

        private static bool CheckTypes( ParameterInfo[] pis, object[] args )
        {
            for ( int i = 0; i < args.Length; i++ )
            {
                Type t = pis[i].ParameterType;
                Type a = args[i].GetType();

                if ( !t.IsAssignableFrom( a ) )
                {
                    return false;
                }
            }

            return true;
        }

        private ABSObject InvokeMethod( MethodInfo mi, object[] args )
        {
            object o = mi.Invoke( m_Instance, args );

            return BSReflectionInterface.Instance.Wrap( o );
        }

        private ABSObject InvokeMethod( ABSObject[] args )
        {
            object[] a = GetObjects( args );

            return InvokeMethod( FindMethod( m_MethodInfos, a ), a );
        }

        #endregion

    }

}
