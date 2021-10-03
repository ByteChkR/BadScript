using System.Linq;

using BadScript.Types;

namespace BadScript.Parser.OperatorImplementations
{

    public class BSOperatorImplementation : ABSOperatorImplementation
    {

        private readonly BSFunction m_Implementation;

        #region Public

        public BSOperatorImplementation( string key, BSFunction impl ) : base( key )
        {
            m_Implementation = impl;
        }

        public override bool IsCorrectImplementation( ABSObject[] args )
        {
            return true;
        }

        #endregion

        #region Protected

        protected override ABSObject Execute( ABSObject[] args )
        {
            return m_Implementation.Invoke( args.Skip( 1 ).ToArray() );
        }

        #endregion

    }

}
