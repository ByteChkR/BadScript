using System;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Utils
{

    internal readonly struct TypePropertyBuilderData: ITypePropertyData
    {
        public static TypePropertyBuilderData Empty => new TypePropertyBuilderData( o => new BSObject( null ), null );
        private readonly Func<object,ABSObject> m_Getter;
        private readonly Action<object,ABSObject> m_Setter;

        public TypePropertyBuilderData( Func < object, ABSObject > get, Action < object, ABSObject > set )
        {
            m_Getter = get;
            m_Setter = set;
        }
        public Func < ABSObject > MakeGetter( object instance )
        {
            Func < object, ABSObject > g = m_Getter;
            return () => g( instance );
        }

        public Action < ABSObject > MakeSetter( object instance )
        {
            Action < object, ABSObject > s = m_Setter;

            return ( o ) => s( instance, o );
        }
    }

}