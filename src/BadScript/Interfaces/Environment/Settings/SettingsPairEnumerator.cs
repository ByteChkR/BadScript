﻿using System.Collections;
using System.Collections.Generic;

using BadScript.Exceptions;
using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Block.ForEach;
using BadScript.Settings;
using BadScript.Types;
using BadScript.Types.References;

namespace BadScript.Interfaces.Environment.Settings
{

    public class SettingsPairEnumerator : ABSObject, IEnumerable < IForEachIteration >
    {

        private readonly SettingsCategory m_Category;

        #region Public

        public SettingsPairEnumerator( SettingsCategory cat ) : base( SourcePosition.Unknown )
        {
            m_Category = cat;
        }

        public override bool Equals( ABSObject other )
        {
            return other is SettingsPairEnumerator w && w.m_Category == m_Category;
        }

        public IEnumerator < IForEachIteration > GetEnumerator()
        {
            foreach ( SettingsPair pair in ( IEnumerable < SettingsPair > )m_Category )
            {
                yield return new ForEachIteration( new ABSObject[] { new SettingsPairWrapper( pair ) } );
            }
        }

        public override ABSReference GetProperty( string propertyName )
        {
            throw new BSRuntimeException( "Settings Pair Enumerator has no Properties" );
        }

        public override bool HasProperty( string propertyName )
        {
            return false;
        }

        public override ABSObject Invoke( ABSObject[] args )
        {
            throw new BSRuntimeException( "Can not Invoke Settings Pair Enumerator" );
        }

        public override bool IsNull()
        {
            return false;
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
        {
            return m_Category.FullName + "(Pair Enumerator)";
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
            v = null;

            return false;
        }

        #endregion

        #region Protected

        protected override int GetHashCodeImpl()
        {
            return m_Category.GetHashCode() ^ typeof( SettingsPairEnumerator ).GetHashCode();
        }

        #endregion

        #region Private

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }

}
