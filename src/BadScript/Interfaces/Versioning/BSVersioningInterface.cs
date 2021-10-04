using System;

using BadScript.Types;
using BadScript.Types.Implementations;

namespace BadScript.Interfaces.Versioning
{

    public class BSVersioningInterface : BSConstantScriptInterface
    {

        #region Public

        public BSVersioningInterface() : base( "Versioning", MakeInterface )
        {
        }

        #endregion

        #region Private

        private static ABSObject CreateCalVer( ABSObject[] arg )
        {
            string fmtString = "(~){yyyy}.(~){MM}.(~){dd}.+";

            return new BSVersionObject(
                                       new Version( 0, 0, 0, ( int )arg[0].ConvertDecimal() ).ChangeVersion( fmtString )
                                      );
        }

        private static ABSObject CreateVersion( ABSObject[] arg )
        {
            return new BSVersionObject(
                                       new Version(
                                                   ( int )arg[0].ConvertDecimal(),
                                                   ( int )arg[1].ConvertDecimal(),
                                                   ( int )arg[2].ConvertDecimal(),
                                                   ( int )arg[3].ConvertDecimal()
                                                  )
                                      );
        }

        private static void MakeInterface( ABSTable obj )
        {
            obj.InsertElement(
                              new BSObject( "Parse" ),
                              new BSFunction("function Parse(versionStr)", ParseVersion, 1 )
                             );

            obj.InsertElement(
                              new BSObject( "Create" ),
                              new BSFunction("function Create(major, minor, revision, build)", CreateVersion, 4 )
                             );

            obj.InsertElement(
                              new BSObject( "RuntimeVersion" ),
                              new BSVersionObject( typeof( BSEngine ).Assembly.GetName().Version )
                             );

            obj.InsertElement( new BSObject( "CalVer" ), new BSFunction("function CalVer(build)", CreateCalVer, 1 ) );
        }

        private static ABSObject ParseVersion( ABSObject[] arg )
        {
            Version v = Version.Parse( arg[0].ConvertString() );

            return new BSVersionObject( v );
        }

        #endregion

    }

}
