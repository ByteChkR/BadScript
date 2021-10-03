using BadScript.Parser.Expressions.Implementations.Access;
using BadScript.Parser.Expressions.Implementations.Value;
using BadScript.Scopes;
using BadScript.Types;
using BadScript.Types.Implementations.Types;

namespace BadScript.Parser.Expressions.Implementations.Types
{

    public class BSNewInstanceExpression : BSExpression
    {

        public BSInvocationExpression Name;

        public override bool IsConstant => false;

        #region Public

        public BSNewInstanceExpression( SourcePosition srcPos, BSInvocationExpression name ) : base( srcPos )
        {
            Name = name;
        }

        public override ABSObject Execute( BSScope scope )
        {
            ABSObject[] args = new ABSObject[Name.Parameters.Length];

            for ( int i = 0; i < Name.Parameters.Length; i++ )
            {
                BSExpression nameParameter = Name.Parameters[i];
                args[i] = nameParameter.Execute( scope );
            }

            return BSTypeDatabase.CreateInstance(
                                                 ( Name.Left as BSPropertyExpression ).Right,
                                                 scope.Engine,
                                                 scope.Namespace,
                                                 args
                                                );
        }

        #endregion

    }

}
