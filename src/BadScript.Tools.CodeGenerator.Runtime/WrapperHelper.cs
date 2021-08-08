using System;
using System.Collections.Generic;
using System.Linq;
using BadScript.Common.Exceptions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Tools.CodeGenerator.Runtime.Attributes;

namespace BadScript.Tools.CodeGenerator.Runtime
{

    public static class WrapperHelper
    {
        private static readonly List <IWrapperConstructorDataBase> s_DataBases =
            new List<IWrapperConstructorDataBase>();

        public static void AddObjectDB(IWrapperConstructorDataBase db)
        {
            if (s_DataBases.Contains(db)) return;

            s_DataBases.Add(db);
        }

        public static BSWrapperObject < T > Create < T >( object[] args )
        {
            IWrapperConstructorDataBase db = s_DataBases.FirstOrDefault( x => x.HasType < T >() );

            if ( db != null )
                return db.Get < T >(args);

            throw new Exception( "Type not Found" );
        }

        public static T UnwrapObject<T>(ABSObject o)
        {
            //TODO Convert Numbers and Strings
            if(o is BSWrapperObject <T> ob )
            {
                return ob.GetInternalObject();
            }
            else if ( o is BSObject obj )
            {
                object oi = obj.GetInternalObject();

                if (typeof(T) == typeof(bool))
                {
                    return (T)(object)o.ConvertBool();
                }
                else if (typeof(T) == typeof(decimal))
                {
                    return (T)(object)o.ConvertDecimal();
                }
                else if (typeof(T) == typeof(string))
                {
                    return (T)(object)o.ConvertString();
                }
                return (T)Convert.ChangeType(oi, typeof(T));
            }
            throw new BSRuntimeException( "Can not Unwrap Object" );
        }
    }

}
