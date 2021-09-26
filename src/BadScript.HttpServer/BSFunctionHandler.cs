using System.Threading.Tasks;

using BadScript.Common.Types;

using Ceen;

namespace BadScript.HttpServer
{

    public class BSFunctionHandler : IHttpModule
    {

        private readonly ABSObject m_Func;

        #region Public

        public BSFunctionHandler( ABSObject o )
        {
            m_Func = o;
        }

        public async Task < bool > HandleAsync( IHttpContext context )
        {
            m_Func.Invoke( new ABSObject[] { new HttpServerContextObject( m_Func.Position, context ) } );

            return true;
        }

        #endregion

    }

}
