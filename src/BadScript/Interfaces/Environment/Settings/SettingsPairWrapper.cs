using System.Collections.Generic;

using BadScript.Exceptions;
using BadScript.Parser.Expressions;
using BadScript.Reflection;
using BadScript.Settings;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Interfaces.Environment.Settings
{

    public class SettingsPairWrapper : ABSObject
    {

        private readonly SettingsPair m_Pair;
        private readonly Dictionary < string, ABSReference > m_Properties;

        public override bool IsNull => false;

        #region Public

        public SettingsPairWrapper( SettingsPair pair ) : base( SourcePosition.Unknown )
        {
            m_Pair = pair;

            m_Properties = new Dictionary < string, ABSReference >
                           {
                               { "Name", new BSReflectionReference( () => new BSObject( m_Pair.Name ), null ) },
                               {
                                   "Value", new BSReflectionReference(
                                                                      () => new BSObject( m_Pair.Value ),
                                                                      x => m_Pair.Value = x.ConvertString()
                                                                     )
                               },
                               {
                                   "IsPersistent", new BSReflectionReference(
                                                                             () => new BSObject( m_Pair.IsPersistent ),
                                                                             x => m_Pair.IsPersistent = x.ConvertBool()
                                                                            )
                               },
                           };
        }

        public override bool Equals( ABSObject other )
        {
            return other is SettingsPairWrapper w && w.m_Pair == m_Pair;
        }

        public override ABSReference GetProperty( string propertyName )
        {
            return m_Properties[propertyName];
        }

        public override bool HasProperty( string propertyName )
        {
            return m_Properties.ContainsKey( propertyName );
        }

        public override ABSObject Invoke( ABSObject[] args )
        {
            throw new BSRuntimeException( "Can not Invoke Settings Category" );
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
        {
            return m_Pair.Name + " : " + m_Pair.Value;
        }

        public override void SetProperty( string propertyName, ABSObject obj )
        {
            throw new BSRuntimeException( "Can not Set Property: " + propertyName );
        }

        public override bool TryConvertBool( out bool v )
        {
            v = false;

            return false;
        }

        public override bool TryConvertDecimal( out decimal d )
        {
            d = 0;

            return false;
        }

        public override bool TryConvertString( out string v )
        {
            v = m_Pair.Value;

            return true;
        }

        #endregion

    }

}
