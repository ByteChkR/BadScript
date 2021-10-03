﻿using System.Linq;

using BadScript.Interfaces;
using BadScript.Parser.Expressions;
using BadScript.Types;
using BadScript.Types.Implementations;

namespace BadScript.Console.AppPackage
{

    public class BSAppPackageInterface : ABSScriptInterface
    {

        private readonly BSAppPackage m_Package;

        #region Public

        public BSAppPackageInterface( BSAppPackage p ) : base( "app" )
        {
            m_Package = p;
        }

        public override void AddApi( ABSTable root )
        {
            root.InsertElement( "Manifest", CreateManifestTable() );
            root.InsertElement( "GetResource", new BSFunction( "function GetResource(path)", GetResource, 1 ) );
            root.InsertElement( "GetItem", new BSFunction( "function GetItem(path)", GetItem, 1 ) );
        }

        #endregion

        #region Private

        private ABSTable CreateManifestTable()
        {
            BSTable table = new BSTable( SourcePosition.Unknown );
            table.InsertElement( "Name", m_Package.Manifest.AppName );
            table.InsertElement( "Version", m_Package.Manifest.AppVersion );
            table.InsertElement( "RuntimeMin", m_Package.Manifest.RuntimeMinVersion );
            table.InsertElement( "RuntimeMax", m_Package.Manifest.RuntimeMaxVersion );
            table.InsertElement( "ExecutablePath", m_Package.Manifest.ExecutablePath );

            table.InsertElement(
                                "RequiredInterfaces",
                                new BSArray( m_Package.Manifest.RequiredInterfaces.Select( x => new BSObject( x ) ) )
                               );

            table.Lock();

            return table;
        }

        private ABSObject GetItem( ABSObject[] arg )
        {
            return new BSStreamObject( m_Package.GetResource( arg[0].ConvertString() ) );
        }

        private ABSObject GetResource( ABSObject[] arg )
        {
            return new BSStreamObject( m_Package.GetResource( arg[0].ConvertString() ) );
        }

        #endregion

    }

}