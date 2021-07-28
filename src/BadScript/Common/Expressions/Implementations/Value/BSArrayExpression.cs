using System.Linq;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.Expressions.Implementations.Value
{

    public class BSArrayExpression : BSExpression
    {
        private readonly BSExpression[] m_InitExpressions;

        #region Public

        public BSArrayExpression( SourcePosition pos, BSExpression[] initExprs = null ) : base( pos )
        {
            m_InitExpressions = initExprs ?? new BSExpression[0];
        }

        public override ABSObject Execute( BSScope scope )
        {
            return new BSArray( m_InitExpressions.Select( x => x.Execute( scope ) ) );
        }

        #endregion
    }

}
