using BadScript.Runtime;
using BadScript.Runtime.Implementations;

namespace BadScript.Parser.Expressions.Value
{

    public class BSWhileExpression : BSExpression
    {

        private BSExpression m_Condition;
        private BSExpression[] m_Block;

        #region Public

        public BSWhileExpression( BSExpression condition, BSExpression[] block )
        {
            m_Condition = condition;
            m_Block = block;
        }

        public override BSRuntimeObject Execute( BSEngineScope scope )
        {
            BSRuntimeObject o = m_Condition.Execute( scope );

            if ( o is BSRuntimeReference r )
            {
                o = r.Get();
            }

            while ( o.TryConvertBool( out bool d ) && d )
            {
                BSEngineScope funcScope = new BSEngineScope( scope );

                BSRuntimeObject ret = BSFunctionDefinitionExpression.InvokeBlockFunction(
                     funcScope,
                     m_Block,
                     new string[0],
                     new BSRuntimeObject[0]
                    );

                if ( ret != null )
                {
                    scope.SetReturnValue( ret );

                    break;
                }

                o = m_Condition.Execute( scope );

                if ( o is BSRuntimeReference rI )
                {
                    o = rI.Get();
                }
            }

            return new EngineRuntimeObject( null );
        }

        #endregion

    }

}
