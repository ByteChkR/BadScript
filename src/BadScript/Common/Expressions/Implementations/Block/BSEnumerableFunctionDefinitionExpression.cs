using System.Text;
using BadScript.Common.Runtime;
using BadScript.Common.Types;

namespace BadScript.Common.Expressions.Implementations.Block
{

    public class BSEnumerableFunctionDefinitionExpression : BSExpression
    {
        public string Name;
        public bool Global;
        public (bool, string)[] ArgNames;
        public BSExpression[] Block;
        public BSEnumerableFunctionDefinitionExpression( SourcePosition pos, string name, bool isGlobal, (bool, string)[] argName, BSExpression[] block ) : base( pos )
        {
            Name = name;
            Global = isGlobal;
            ArgNames = argName;
            Block = block;
        }

        public override bool IsConstant => false;
        private  (string, ABSObject)[] CreateArgs(ABSObject[] o)
        {
            (string, ABSObject)[] r = new (string, ABSObject)[o.Length];

            for ( int i = 0; i < ArgNames.Length; i++ )
            {
                (bool, string) valueTuple = ArgNames[i];
                r[i] = ( valueTuple.Item2, o[i] );
            }

            return r;
        }
        public override ABSObject Execute( BSScope scope )
        {
            BSScope funcScope = new BSScope(BSScopeFlags.Function, scope);

            BSFunction f= new BSFunction(
                m_Position,
                GetHeader(),
                x => 
                    new BSFunctionEnumeratorObject( m_Position, GetHeader(), Block, CreateArgs( x ), funcScope ),
                ArgNames.Length );

            if (string.IsNullOrEmpty(Name))
            {
                return f;
            }

            if (Global)
            {
                scope.AddGlobalVar(Name, f);
            }
            else
            {
                scope.AddLocalVar(Name, f);
            }

            return f;
            //return new BSFunctionEnumeratorObject(m_Position, GetHeader(), Block, );
        }

        private string GetHeader()
        {
            StringBuilder sb = new StringBuilder($"enumerable {Name}(");

            for (int i = 0; i < ArgNames.Length; i++)
            {
                string argName = ArgNames[i].Item2;

                if (!ArgNames[i].Item1)
                {
                    argName = "!" + argName;
                }

                if (i == 0)
                {
                    sb.Append(argName);
                }
                else
                {
                    sb.Append(", " + argName);
                }
            }

            sb.Append(")");

            return sb.ToString();
        }
    }

}