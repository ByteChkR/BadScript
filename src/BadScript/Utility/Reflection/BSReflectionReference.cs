using System;

using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Types;
using BadScript.Common.Types.References;

namespace BadScript.Utility.Reflection
{

    public class BSReflectionReference : ABSReference
    {

        private readonly Func < ABSObject > m_Getter;
        private readonly Action < ABSObject > m_Setter;

        #region Public

        public BSReflectionReference( Func < ABSObject > get, Action < ABSObject > set ) : base(
             SourcePosition.Unknown
            )
        {
            m_Getter = get;
            m_Setter = set;
        }

        public override void Assign( ABSObject obj )
        {
            if ( m_Setter == null )
            {
                throw new BSRuntimeException( "Reflection Reference is Readonly" );
            }

            m_Setter( obj );
        }

        public override ABSObject Get()
        {
            return m_Getter();
        }

        #endregion

    }

}
