using System.Collections.Generic;
using System.Linq;

using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.Expressions.Implementations.Value
{

    public class BSClassExpression : BSExpression
    {
        public readonly Dictionary<string, BSExpression> InitExpressions;
        public readonly string Name;
        public readonly string BaseName;

        public override bool IsConstant => InitExpressions.All(x => x.Value.IsConstant);

        #region Public

        public BSClassExpression(SourcePosition pos, string name, string baseName, Dictionary<string, BSExpression> initExprs = null) : base(pos)
        {
            Name = name;
            BaseName = baseName;
            InitExpressions = initExprs ?? new Dictionary<string, BSExpression>();
            BSClassBase.AddClass( this );
        }

        public void AddClassData(  BSScope scope )
        {
            //BSScope instanceScope = new BSScope(BSScopeFlags.Return, scope);

            foreach (KeyValuePair<string, BSExpression> initExpression in InitExpressions)
            {
                scope.AddLocalVar( initExpression.Key, initExpression.Value.Execute( scope ) );
            }
        }

        public override ABSObject Execute(BSScope scope)
        {
            return BSObject.Null;
        }

        #endregion
    }

}