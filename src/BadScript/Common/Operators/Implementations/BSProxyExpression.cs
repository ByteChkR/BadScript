using BadScript.Common.Expressions;
using BadScript.Common.Runtime;
using BadScript.Common.Types;

namespace BadScript.Common.Operators.Implementations
{

    public class BSProxyExpression : BSExpression
    {
        private ABSObject m_Object;

        #region Public

        public BSProxyExpression( SourcePosition pos, ABSObject obj ) : base( pos )
        {
            m_Object = obj;
        }

        public override ABSObject Execute( BSScope scope )
        {
            return m_Object;
        }

        #endregion
    }

}
