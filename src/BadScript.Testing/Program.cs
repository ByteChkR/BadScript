using BadScript.Settings;

namespace BadScript.Testing
{

    public static class Program
    {

        #region Public

        public static void Main( string[] args )
        {
            BSEngineSettings es = BSEngineSettings.MakeDefault();
            BSEngine e = es.Build();
            e.LoadFile( "D:\\Users\\Tim\\Documents\\BadScript\\src\\BadScript.Testing\\ReturnValidatorTest.bs" );
        }

        #endregion

    }

}
