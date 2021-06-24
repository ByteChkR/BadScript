using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.Expressions.Implementations.Value
{

    public class BSValueExpression : BSExpression
    {
        private readonly object m_Value;

        #region Public

        public BSValueExpression( object o )
        {
            m_Value = o;
        }

        public override ABSObject Execute( BSScope scope )
        {
            ABSObject ret = new BSObject( m_Value );

            return ret;
        }

        #endregion
    }

}
