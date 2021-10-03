using System;
using System.Collections.Generic;
using System.Linq;

using BadScript.Parser.Expressions.Implementations.Access;
using BadScript.Parser.Expressions.Implementations.Block;
using BadScript.Parser.Expressions.Implementations.Value;
using BadScript.Scopes;
using BadScript.Types;
using BadScript.Types.Implementations;

namespace BadScript.Parser.Expressions.Implementations.Types
{

    public class BSClassExpression : BSExpression
    {

        public readonly Dictionary < string, BSExpression > InitExpressions;
        public readonly string Name;
        public readonly string BaseName;
        public readonly bool IsGlobal;

        public override bool IsConstant => InitExpressions.All( x => x.Value.IsConstant );

        public BSScope DefiningScope { get; private set; }

        #region Public

        public BSClassExpression(
            SourcePosition pos,
            string name,
            string baseName,
            bool isGlobal,
            Dictionary < string, BSExpression > initExprs = null ) : base( pos )
        {
            Name = name;
            BaseName = baseName;
            IsGlobal = isGlobal;
            InitExpressions = initExprs ?? new Dictionary < string, BSExpression >();

            //Add Default Functions
            //  GetType => Function that returns "Name"
            //  ToString => Function that calls GetType()
            //  IsInstanceOf(string) => Function that calls 
            if ( !InitExpressions.ContainsKey( "GetType" ) )
            {
                BSExpression expr = new BSFunctionDefinitionExpression(
                                                                       SourcePosition.Unknown,
                                                                       "GetType",
                                                                       Array.Empty < BSFunctionParameter >(),
                                                                       new BSExpression[]
                                                                       {
                                                                           new BSReturnExpression(
                                                                                SourcePosition.Unknown,
                                                                                new BSValueExpression(
                                                                                     SourcePosition.Unknown,
                                                                                     Name
                                                                                    )
                                                                               )
                                                                       },
                                                                       false
                                                                      );

                InitExpressions["GetType"] = expr;
            }

            if ( !InitExpressions.ContainsKey( "ToString" ) )
            {
                BSExpression expr = new BSFunctionDefinitionExpression(
                                                                       SourcePosition.Unknown,
                                                                       "ToString",
                                                                       Array.Empty < BSFunctionParameter >(),
                                                                       new BSExpression[]
                                                                       {
                                                                           new BSReturnExpression(
                                                                                SourcePosition.Unknown,
                                                                                new BSInvocationExpression(
                                                                                     SourcePosition.Unknown,
                                                                                     new BSPropertyExpression(
                                                                                          SourcePosition.Unknown,
                                                                                          null,
                                                                                          "GetType"
                                                                                         ),
                                                                                     Array.Empty < BSExpression >()
                                                                                    )
                                                                               )
                                                                       },
                                                                       false
                                                                      );

                InitExpressions["ToString"] = expr;
            }
        }

        public void AddClassData( BSScope scope )
        {
            foreach ( KeyValuePair < string, BSExpression > initExpression in InitExpressions )
            {
                scope.AddLocalVar( initExpression.Key, initExpression.Value.Execute( scope ) );
            }
        }

        public override ABSObject Execute( BSScope scope )
        {
            DefiningScope = scope;

            if ( IsGlobal )
            {
                scope.Engine.NamespaceRoot.AddType( this );
            }
            else
            {
                scope.Namespace.AddType( this );
            }

            return BSObject.Null;
        }

        #endregion

    }

}
