using System;
using System.Collections.Generic;
using System.Linq;

using BadScript.Common.Expressions.Implementations.Access;
using BadScript.Common.Expressions.Implementations.Block;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.Expressions.Implementations.Value
{

    public class BSClassExpression : BSExpression
    {

        public readonly Dictionary < string, BSExpression > InitExpressions;
        public readonly string Name;
        public readonly string BaseName;

        public override bool IsConstant => InitExpressions.All( x => x.Value.IsConstant );

        #region Public

        public BSClassExpression(
            SourcePosition pos,
            string name,
            string baseName,
            Dictionary < string, BSExpression > initExprs = null ) : base( pos )
        {
            Name = name;
            BaseName = baseName;
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
            scope.Engine.TypeDatabase.AddClass( this );

            return BSObject.Null;
        }

        #endregion

    }

}
