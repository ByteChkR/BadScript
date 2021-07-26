using System.Threading.Tasks;
using BadScript.Common.Expressions;
using BadScript.Common.Types;
using Ceen;

namespace BadScript.Https
{
    public class BSFunctionHandler : IHttpModule
    {
        private readonly ABSObject m_Func;
        public BSFunctionHandler(ABSObject o)
        {
            m_Func = o;
        }
        public async Task<bool> HandleAsync(IHttpContext context)
        {
            m_Func.Invoke(new ABSObject[]
            {
                new HttpServerContextObject(SourcePosition.Unknown, context)
            });
            return true;
        }
    }
}