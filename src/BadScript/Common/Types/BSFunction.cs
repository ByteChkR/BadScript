using System;
using System.Collections.Generic;
using BadScript.Common.Exceptions;
using BadScript.Common.Types.References;

namespace BadScript.Common.Types
{

    public class BSFunction : ABSObject
    {
        private readonly (int min, int max)? m_ParameterCount;
        private readonly string m_DebugData = null;

        private Func < ABSObject[],
            ABSObject > m_Func;

        public override bool IsNull => false;

        #region Public

        public BSFunction(
            string debugData,
            Func < ABSObject[], ABSObject >
                func,
            int argCount ) : this( debugData, func )
        {
            m_ParameterCount = ( argCount, argCount );
        }

        public BSFunction(
            string debugData,
            Func < ABSObject[], ABSObject >
                func,
            int minArgs,
            int maxArgs ) : this( debugData, func )
        {
            m_ParameterCount = ( minArgs, maxArgs );
        }

        public override bool Equals( ABSObject other )
        {
            return ReferenceEquals( this, other );
        }

        public override ABSReference GetProperty( string propertyName )
        {
            throw new BSRuntimeException( $"Property {propertyName} does not exist" );
        }

        public override bool HasProperty( string propertyName )
        {
            return false;
        }

        public override ABSObject Invoke( ABSObject[] args )
        {
            if ( m_ParameterCount == null )
            {
                return m_Func( args );
            }

            ( int min, int max ) = m_ParameterCount.Value;

            if ( args.Length < min || args.Length > max )
            {
                throw new BSRuntimeException(
                    $"Invalid parameter Count: '{m_DebugData}' expected {min} - {max} and got {args.Length}" );
            }

            return m_Func( args );
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
        {
            return m_DebugData ?? m_Func.ToString();
        }

        public override void SetProperty( string propertyName, ABSObject obj )
        {
            throw new BSRuntimeException( $"Property {propertyName} does not exist" );
        }

        public override bool TryConvertBool( out bool v )
        {
            v = true;

            return true;
        }

        public override bool TryConvertDecimal( out decimal d )
        {
            d = 1;

            return false;
        }

        public override bool TryConvertString( out string v )
        {
            v = m_DebugData ?? m_Func.ToString();
            ;

            return true;
        }

        #endregion

        #region Private

        private BSFunction(
            string debugData,
            Func < ABSObject[], ABSObject >
                func )
        {
            m_DebugData = debugData;
            m_Func = func;
        }

        #endregion
    }

}
