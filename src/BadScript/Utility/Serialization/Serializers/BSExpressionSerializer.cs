﻿using System.IO;

using BadScript.Common.Expressions;

namespace BadScript.Utility.Serialization.Serializers
{

    public abstract class BSExpressionSerializer
    {

        #region Public

        public abstract bool CanDeserialize( BSCompiledExpressionCode code );

        public abstract bool CanSerialize( BSExpression expr );

        public abstract BSExpression Deserialize( BSCompiledExpressionCode code, Stream s, BSSerializerContext context );

        public abstract void Serialize( BSExpression expr, Stream s, BSSerializerContext context);

        #endregion

    }

}
