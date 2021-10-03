using BadScript.Parser.Expressions;
using BadScript.Scopes;
using BadScript.Types;

namespace BadScript.Parser.Operators.Implementations
{

    public class BSProxyExpression : BSExpression
    {

        public readonly ABSObject Object;
        public readonly object ProxyMetaData;

        public override bool IsConstant => true;

        #region Public

        public BSProxyExpression( SourcePosition pos, ABSObject obj, object proxyMetaData = null ) : base( pos )
        {
            Object = obj;
            ProxyMetaData = proxyMetaData;
        }

        public override ABSObject Execute( BSScope scope )
        {
            return Object;
        }

        #endregion

    }

}
