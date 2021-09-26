﻿using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Access;

namespace BadScript.Common.Operators.Implementations
{

    public class BSNullCheckedMemberAccessOperator : BSOperator
    {

        public override string OperatorKey => "?.";

        public override int Preceedence => 2;

        #region Public

        public override BSExpression Parse( BSExpression left, BSParser parser )
        {
            parser.ReadWhitespaceAndNewLine();
            string wordName = parser.GetNextWord();

            return new BSNullCheckPropertyExpression( parser.CreateSourcePosition(), left, wordName );
        }

        #endregion

    }

}
