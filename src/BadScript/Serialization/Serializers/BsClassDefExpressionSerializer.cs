using System.Collections.Generic;
using System.IO;

using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Types;

namespace BadScript.Serialization.Serializers
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

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s, BSSerializerContext context )
        {
            string name = s.DeserializeString( context );
            bool isGlobal = s.DeserializeBool();
            bool hasBase = s.DeserializeBool();
            string baseName = null;

            if ( hasBase )
            {
                baseName = s.DeserializeString( context );
            }

            Dictionary < string, BSExpression > initExpressions = s.DeserializeNameMap( context );

            return new BSClassExpression( SourcePosition.Unknown, name, baseName, isGlobal, initExpressions );
        }

        public override void Serialize( BSExpression expr, Stream s, BSSerializerContext context )
        {
            BSClassExpression b = ( BSClassExpression )expr;
            s.SerializeOpCode( BSCompiledExpressionCode.ClassDefExpr );
            s.SerializeString( b.Name, context );
            s.SerializeBool( b.IsGlobal );
            bool hasBase = b.BaseName != null;
            s.SerializeBool( hasBase );

            if ( hasBase )
            {
                s.SerializeString( b.BaseName, context );
            }

            s.SerializeNameMap( b.InitExpressions, context );
        }

        #endregion

    }

}
