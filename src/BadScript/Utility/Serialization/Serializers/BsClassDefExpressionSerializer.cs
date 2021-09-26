using System.Collections.Generic;
using System.IO;

using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Value;

namespace BadScript.Utility.Serialization.Serializers
{

    public class BsClassDefExpressionSerializer : BSExpressionSerializer
    {

        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.ClassDefExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSClassExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            string name = s.DeserializeString();
            bool hasBase = s.DeserializeBool();
            string baseName = null;

            if ( hasBase )
            {
                baseName = s.DeserializeString();
            }

            Dictionary < string, BSExpression > initExpressions = s.DeserializeNameMap();

            return new BSClassExpression( SourcePosition.Unknown, name, baseName, initExpressions );
        }

        public override void Serialize( BSExpression expr, Stream s )
        {
            BSClassExpression b = ( BSClassExpression )expr;
            s.SerializeOpCode( BSCompiledExpressionCode.ClassDefExpr );
            s.SerializeString( b.Name );
            bool hasBase = b.BaseName != null;
            s.SerializeBool( hasBase );

            if ( hasBase )
            {
                s.SerializeString( b.BaseName );
            }

            s.SerializeNameMap( b.InitExpressions );
        }

        #endregion

    }

}
