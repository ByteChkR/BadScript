using System.Collections.Generic;

namespace BadScript.Types.Implementations
{

    public class BSObjectExtension
    {

        private readonly Dictionary < string, BSFunction > m_Properties;

        #region Public

        public BSObjectExtension() : this( new Dictionary < string, BSFunction >() )
        {
        }

        public BSObjectExtension( Dictionary < string, BSFunction > properties )
        {
            m_Properties = properties;
        }

        public ABSObject GetProperty( string p, BSObject o )
        {
            return MakeInstance( m_Properties[p], o );
        }

        public bool HasProperty( string p )
        {
            return m_Properties.ContainsKey( p );
        }

        #endregion

        #region Private

        private BSFunction MakeInstance( BSFunction f, BSObject o )
        {
            return new BSFunction(
                                  f.DebugData,
                                  args =>
                                  {
                                      ABSObject[] a = new ABSObject[args.Length + 1];
                                      a[0] = o;

                                      for ( int i = 0; i < args.Length; i++ )
                                      {
                                          a[i + 1] = args[i];
                                      }

                                      return f.Invoke( a );
                                  },
                                  f.MinParameters - 1,
                                  f.MaxParameters - 1
                                 );
        }

        #endregion

    }

}
