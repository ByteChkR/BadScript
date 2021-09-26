using BadScript.Common.Expressions.Implementations.Access;
using BadScript.Common.Expressions.Implementations.Unary;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.Expressions.Implementations.Value
{

    public class BSReturnExpression : BSUnaryExpression
    {
        public override bool IsConstant => false;

        #region Public

        public BSReturnExpression(SourcePosition srcPos, BSExpression left) : base(srcPos, left)
        {
        }

        public override ABSObject Execute(BSScope scope)
        {
            ABSObject o = Left.Execute(scope);

            scope.SetFlag(BSScopeFlags.Return, o);

            return o;
        }

        #endregion
    }

    public class BSNewInstanceExpression : BSExpression
    {
        public override bool IsConstant => false;

        private readonly BSInvocationExpression Name;
        #region Public

        public BSNewInstanceExpression(SourcePosition srcPos, BSInvocationExpression name) : base(srcPos)
        {
            Name = name;
        }

        public override ABSObject Execute(BSScope scope)
        {
            ABSObject[] args = new ABSObject[Name.Parameters.Length];

            for ( int i = 0; i < Name.Parameters.Length; i++ )
            {
                BSExpression nameParameter = Name.Parameters[i];
                args[i] = nameParameter.Execute( scope );
            }

            return BSClassBase.CreateInstance( (Name.Left as BSPropertyExpression).Right, scope.Engine, args );
        }

        #endregion
    }

}
