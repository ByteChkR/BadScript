using System;

using BadScript.Runtime;
using BadScript.Runtime.Implementations;

namespace BadScript.Parser.Expressions.Value
{

    public class BSValueExpression : BSExpression
    {
        
        private readonly object m_Value;

        #region Public

        public BSValueExpression(  object o )
        {
            m_Value = o;
        }

        public override BSRuntimeObject Execute( BSEngineScope scope )
        {
            BSRuntimeObject ret = new EngineRuntimeObject( m_Value );
            return ret;
        }

        #endregion

    }

}
