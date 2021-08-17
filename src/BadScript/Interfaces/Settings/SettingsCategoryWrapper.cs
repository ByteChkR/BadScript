﻿using System.Collections;
using System.Collections.Generic;
using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block.ForEach;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;
using BadScript.Utils;
using BadScript.Utils.Reflection;

namespace BadScript.Interfaces.Settings
{

    public class SettingsCategoryWrapper:ABSObject
    {
        private readonly SettingsCategory m_Category;
        private readonly SettingsPairEnumerator m_PairEnumerator;
        private readonly SettingsCategoryEnumerator m_CategoryEnumerator;
        private readonly Dictionary < string, ABSReference > m_Properties;
        public SettingsCategoryWrapper( SettingsCategory cat ) : base( SourcePosition.Unknown )
        {
            m_Category = cat;
            m_PairEnumerator = new SettingsPairEnumerator( m_Category );
            m_CategoryEnumerator = new SettingsCategoryEnumerator( m_Category );
            m_Properties = new Dictionary < string, ABSReference>
            {
                { "Name", new BSReflectionReference(() => new BSObject(m_Category.Name), null)},
                { "FullName", new BSReflectionReference(() => new BSObject(m_Category.FullName), null)},
                { "IsPersistent", new BSReflectionReference(() => new BSObject(m_Category.IsPersistent?BSObject.One:BSObject.Zero), x=> x.ConvertBool())},
                {"HasSetting", new BSFunctionReference(new BSFunction("function HasSetting(name)", objects =>  new BSObject(m_Category.HasSetting(objects[0].ConvertString())?BSObject.One:BSObject.Zero), 1))},
                {"HasCategory", new BSFunctionReference(new BSFunction("function HasCategory(name)", objects =>  new BSObject(m_Category.HasCategory(objects[0].ConvertString())?BSObject.One:BSObject.Zero), 1))},
                {"GetSetting", new BSFunctionReference(new BSFunction("function GetSetting(name)", objects =>  new SettingsPairWrapper(m_Category.GetSetting(objects[0].ConvertString())), 1))},
                {"AddSetting", new BSFunctionReference(new BSFunction("function AddSetting(name, value)", objects =>  new SettingsPairWrapper(m_Category.SetSetting(objects[0].ConvertString(), objects[1].ConvertString())), 2))},
                {"AddCategory", new BSFunctionReference(new BSFunction("function AddCategory(name)", objects =>  new SettingsCategoryWrapper(m_Category.AddCategory(objects[0].ConvertString())), 1))},
                {"GetCategory", new BSFunctionReference(new BSFunction("function GetCategory(name)", objects =>  new SettingsCategoryWrapper(m_Category.GetCategory(objects[0].ConvertString())), 1))},
                {"Pairs", new BSReflectionReference(() => m_PairEnumerator, null)},
                {"Categories", new BSReflectionReference(() => m_CategoryEnumerator, null)},
            };
        }

        public override bool IsNull => false;

        public override bool Equals( ABSObject other )
        {
            return other is SettingsCategoryWrapper w && w.m_Category == m_Category;
        }

        public override ABSReference GetProperty( string propertyName )
        {
            return m_Properties[propertyName];
        }

        public override bool HasProperty( string propertyName )
        {
            return m_Properties.ContainsKey(propertyName);
        }

        public override ABSObject Invoke( ABSObject[] args )
        {
            throw new BSRuntimeException( "Can not Invoke Settings Category" );
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
        {
            return m_Category.FullName;
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
            v = SafeToString();

            return true;
        }
    }

}
