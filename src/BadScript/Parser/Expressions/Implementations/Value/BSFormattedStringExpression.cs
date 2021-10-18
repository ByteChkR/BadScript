using System.Linq;

using BadScript.Parser.OperatorImplementations.Implementations.Math;
using BadScript.Scopes;
using BadScript.Types;
using BadScript.Types.Implementations;

namespace BadScript.Parser.Expressions.Implementations.Value
{

    public class BSFormattedStringExpression : BSExpression
    {

        public BSExpression[] Args;
        public string FormatString;

        public override bool IsConstant => Args.All( x => x.IsConstant );

        #region Public

        public BSFormattedStringExpression( SourcePosition srcPos, string s, BSExpression[] args ) : base( srcPos )
        {
            FormatString = s;
            Args = args;
        }

        public override ABSObject Execute( BSScope scope )
        {
            string str = string.Format(
                                       FormatString,
                                       Args.Select( x => ( object )Unpack( x.Execute( scope ) ) ).ToArray()
                                      );

            return new BSObject( str );
        }

        #endregion

        #region Private

        private static string Unpack( ABSObject o )
        {
            BSObject str = BSObject.EmptyString;
            ABSObject[] args = { BSObject.EmptyString, o };

            return BSAddOperatorImplementation.Add( args ).ConvertString();
        }

        #endregion

    }

}
