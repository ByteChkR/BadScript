using System;

namespace BadScript.Examples.Integration
{

    public class Greeter
    {

        public string Name;

        #region Public

        public void Greet()
        {
            if ( string.IsNullOrEmpty( Name ) )
            {
                Console.WriteLine( "Hello User!" );
            }
            else
            {
                Console.WriteLine( $"Hello {Name}!" );
            }
        }

        #endregion

    }

}
