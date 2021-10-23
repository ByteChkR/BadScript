using BadScript.Scopes;
using BadScript.Types;
using BadScript.Types.Implementations;

namespace BadScript.Parser.Expressions.Implementations.Value
{

    public class BSValueExpression : BSExpression
    {

        private readonly BSObject m_Value;

        public object SourceValue { get; }

        public override bool IsConstant => true;

        #region Public

        public BSValueExpression( SourcePosition srcPos, object o ) : base( srcPos )
        {
            if ( o is BSObject bso )
            {
                SourceValue = bso.GetInternalObject();
                m_Value = bso;
            }
            else
            {
                SourceValue = o;
                m_Value = new BSObject(m_Position, o );
            }
        }

        public override ABSObject Execute( BSScope scope )
        {
            return m_Value;
        }

        #endregion

    }

}
