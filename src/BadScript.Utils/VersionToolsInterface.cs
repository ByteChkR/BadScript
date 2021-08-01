using System;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Utils
{

    public class VersionToolsInterface : BSConstantScriptInterface
    {
        #region Public

        public VersionToolsInterface() : base( "versioning", MakeInterface )
        {
        }

        #endregion

        #region Private

        private static ABSObject CreateCalVer( ABSObject[] arg )
        {
            string fmtString = "(~){yyyy}.(~){MM}.(~){dd}.+";

            return new BSVersionObject(
                new Version( 0, 0, 0, ( int ) arg[0].ConvertDecimal() ).ChangeVersion( fmtString ) );
        }

        private static ABSObject CreateVersion( ABSObject[] arg )
        {
            return new BSVersionObject(
                new Version(
                    ( int ) arg[0].ConvertDecimal(),
                    ( int ) arg[1].ConvertDecimal(),
                    ( int ) arg[2].ConvertDecimal(),
                    ( int ) arg[3].ConvertDecimal() ) );
        }

        private static void MakeInterface( ABSTable obj )
        {
            obj.InsertElement(
                new BSObject( "parse" ),
                new BSFunction( "function parse(versionStr)", ParseVersion, 1 ) );

            obj.InsertElement(
                new BSObject( "create" ),
                new BSFunction( "function create(major, minor, revision, build)", CreateVersion, 4 ) );

            obj.InsertElement( new BSObject( "calVer" ), new BSFunction( "function calVer(build)", CreateCalVer, 1 ) );
        }

        private static ABSObject ParseVersion( ABSObject[] arg )
        {
            Version v = Version.Parse( arg[0].ConvertString() );

            return new BSVersionObject( v );
        }

        #endregion
    }

}
