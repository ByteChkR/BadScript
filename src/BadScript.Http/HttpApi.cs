﻿using System;
using System.IO;
using System.Net;
using System.Text;

using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block.ForEach;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;
using BadScript.Interfaces;

namespace BadScript.Http
{

    public class HttpApi : ABSScriptInterface
    {

        #region Public

        public HttpApi() : base( "http" )
        {
        }

        public override void AddApi( ABSTable t )
        {
            t.InsertElement(
                            new BSObject( "get" ),
                            new BSFunction( "function get(url)/get(url, headers)", Get, 1, 2 )
                           );

            t.InsertElement(
                            new BSObject( "post" ),
                            new BSFunction( "function post(url, body)", Post, 2 )
                           );

            t.InsertElement(
                            new BSObject( "downloadFile" ),
                            new BSFunction( "function downloadFile(url, destination)", DownloadFile, 2 )
                           );

            t.InsertElement(
                            new BSObject( "downloadString" ),
                            new BSFunction( "function downloadString(url)", DownloadString, 1 )
                           );

            t.InsertElement(
                            new BSObject( "createUri" ),
                            new BSFunction( "function createUri(url)", CreateUri, 1 )
                           );
        }

        #endregion

        #region Private

        private static ABSObject DownloadFile( ABSObject[] args )
        {
            string url = args[0].ResolveReference().ConvertString();
            string file = args[1].ResolveReference().ConvertString();

            using ( WebClient wc = new WebClient() )
            {
                wc.DownloadFile( url, file );
            }

            return BSObject.Null;
        }

        private static ABSObject DownloadString( ABSObject[] args )
        {
            string url = args[0].ResolveReference().ConvertString();

            using ( WebClient wc = new WebClient() )
            {
                return new BSObject( wc.DownloadString( url ) );
            }
        }

        private static ABSObject Get( ABSObject[] args )
        {
            string url = args[0].ResolveReference().ConvertString();

            HttpWebRequest request = WebRequest.CreateHttp( url );

            if ( args.Length == 2 )
            {
                BSTable headers = ( BSTable )args[1].ResolveReference();

                foreach ( IForEachIteration forEachIteration in headers )
                {
                    ABSObject[] kvp = forEachIteration.GetObjects();
                    request.Headers.Add( kvp[0].ConvertString(), kvp[1].ConvertString() );
                }
            }

            request.Credentials = CredentialCache.DefaultCredentials;

            HttpWebResponse response = ( HttpWebResponse )request.GetResponse();
            BSTable t = new BSTable( SourcePosition.Unknown );

            t.InsertElement(
                            new BSObject( "status" ),
                            new BSObject( ( decimal )response.StatusCode )
                           );

            using ( TextReader tr = new StreamReader( response.GetResponseStream() ) )
            {
                t.InsertElement( new BSObject( "body" ), new BSObject( tr.ReadToEnd() ) );
            }

            return t;
        }

        private static ABSObject Post( ABSObject[] args )
        {
            string url = args[0].ResolveReference().ConvertString();
            HttpWebRequest request = WebRequest.CreateHttp( url );
            request.Method = "POST";
            request.Credentials = CredentialCache.DefaultCredentials;

            string dataStr = args[1].ConvertString();
            byte[] data = Encoding.UTF8.GetBytes( dataStr );

            using ( Stream stream = request.GetRequestStream() )
            {
                stream.Write( data, 0, data.Length );
            }

            HttpWebResponse response = ( HttpWebResponse )request.GetResponse();
            BSTable t = new BSTable( SourcePosition.Unknown );

            t.InsertElement(
                            new BSObject( "status" ),
                            new BSObject( ( decimal )response.StatusCode )
                           );

            using ( TextReader tr = new StreamReader( response.GetResponseStream() ) )
            {
                t.InsertElement( new BSObject( "body" ), new BSObject( tr.ReadToEnd() ) );
            }

            return t;
        }

        private ABSObject CreateUri( ABSObject[] arg )
        {
            Uri uri = new Uri( arg[0].ConvertString() );
            BSTable table = new BSTable( SourcePosition.Unknown );

            table.InsertElement(
                                new BSObject( "getHost" ),
                                new BSFunction( "function getHost()", objects => new BSObject( uri.Host ), 0 )
                               );

            table.InsertElement(
                                new BSObject( "getLocalPath" ),
                                new BSFunction( "function getLocalPath()", objects => new BSObject( uri.LocalPath ), 0 )
                               );

            table.InsertElement(
                                new BSObject( "getAuthority" ),
                                new BSFunction( "function getAuthority()", objects => new BSObject( uri.Authority ), 0 )
                               );

            table.InsertElement(
                                new BSObject( "getScheme" ),
                                new BSFunction( "function getScheme()", objects => new BSObject( uri.Scheme ), 0 )
                               );

            table.InsertElement(
                                new BSObject( "getUserInfo" ),
                                new BSFunction( "function getUserInfo()", objects => new BSObject( uri.UserInfo ), 0 )
                               );

            return table;
        }

        #endregion

    }

}
