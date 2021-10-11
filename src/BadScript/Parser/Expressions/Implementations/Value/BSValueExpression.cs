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

        public BSFormattedStringExpression( SourcePosition srcPos, string s, BSExpression[] args ) : base( srcPos )
        {
            FormatString = s;
            Args = args;
        }

        private static string Unpack( ABSObject o )
        {
            BSObject str = BSObject.EmptyString;
            ABSObject[] args = { BSObject.EmptyString, o };

            return BSAddOperatorImplementation.Add( args ).ConvertString();
        }

        public override ABSObject Execute( BSScope scope )
        {
            string str = string.Format( FormatString, Args.Select( x => (object)Unpack(x.Execute(scope)) ).ToArray() );
            return new BSObject( str );
        }

    }

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
                m_Value = new BSObject( o );
            }
        }

        public override ABSObject Execute( BSScope scope )
        {
            return m_Value;
        }

        #endregion

    }

}
