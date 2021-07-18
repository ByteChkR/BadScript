using BadScript.Common.Runtime;
using BadScript.Common.Types;

namespace BadScript.Common.Expressions
{

    public abstract class BSExpression
    {
        protected readonly SourcePosition m_Position;

        #region Public

        public abstract ABSObject Execute( BSScope scope );

        #endregion

        #region Protected

        protected BSExpression( SourcePosition pos )
        {
            m_Position = pos;
        }

        #endregion
    }

}
