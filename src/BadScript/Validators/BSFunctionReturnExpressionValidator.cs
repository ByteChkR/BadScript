using System.Collections.Generic;
using System.Linq;
using System.Text;

using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Block;
using BadScript.Parser.Expressions.Implementations.Block.ForEach;
using BadScript.Parser.Expressions.Implementations.Value;

namespace BadScript.Validators
{

    public class BSFunctionReturnExpressionValidator : BSExpressionValidator
    {

        private struct ReturnInfo
        {

            public bool IsEndBranch;
            public bool HasLocalReturn;

            public bool NoneHaveReturn => !HasLocalReturn && SubBranches.All( x => x.NoneHaveReturn );

            public bool AllHaveReturn =>
                HasLocalReturn && IsEndBranch ||
                IsEndBranch && SubBranches.Count != 0 && SubBranches.All( x => x.AllHaveReturn );

            public List < ReturnInfo > SubBranches;

            public bool Validate()
            {
                return AllHaveReturn || NoneHaveReturn;
            }

            public ReturnInfo( bool hasLocalReturn )
            {
                HasLocalReturn = hasLocalReturn;
                IsEndBranch = true;
                SubBranches = new List < ReturnInfo >();
            }

        }

        public override string ValidatorName => "Return Validation";

        #region Public

        public override void Validate( BSExpression[] expressions )
        {
            List < BSValidatorException > exceptions = new List < BSValidatorException >();
            Validate( null, expressions, exceptions );

            if ( exceptions.Count != 0 )
            {
                StringBuilder sb = new StringBuilder( $"Invalid Functions({exceptions.Count}):\n" );

                foreach ( BSValidatorException bsValidatorException in exceptions )
                {
                    sb.AppendLine( "\t" + bsValidatorException.Message );
                }

                throw new BSValidatorException( sb.ToString(), expressions );
            }
        }

        #endregion

        #region Private

        private ReturnInfo CheckReturnValues( BSExpression[] block, List < BSValidatorException > exceptions )
        {
            ReturnInfo info = new ReturnInfo( false );

            foreach ( BSExpression bsExpression in block )
            {
                if ( bsExpression is BSReturnExpression )
                {
                    info.HasLocalReturn = true;
                    info.SubBranches.Clear();

                    return info;
                }
                else if ( bsExpression is BSForeachExpression foreachExpression )
                {
                    ReturnInfo fInfo = CheckReturnValues( foreachExpression.Block, exceptions );
                    fInfo.IsEndBranch = false;
                    info.SubBranches.Add( fInfo );
                }
                else if ( bsExpression is BSForExpression forExpression )
                {
                    ReturnInfo fInfo = CheckReturnValues( forExpression.Block, exceptions );
                    fInfo.IsEndBranch = false;
                    info.SubBranches.Add( fInfo );
                }
                else if ( bsExpression is BSWhileExpression whileExpression )
                {
                    ReturnInfo wInfo = CheckReturnValues( whileExpression.Block, exceptions );
                    wInfo.IsEndBranch = false;
                    info.SubBranches.Add( wInfo );
                }
                else if ( bsExpression is BSTryExpression tryExpression )
                {
                    info.SubBranches.Add( CheckReturnValues( tryExpression.TryBlock, exceptions ) );
                    info.SubBranches.Add( CheckReturnValues( tryExpression.CatchBlock, exceptions ) );
                }
                else if ( bsExpression is BSIfExpression ifExpression )
                {
                    ReturnInfo ifInfo = new ReturnInfo( false );

                    foreach ( KeyValuePair < BSExpression, BSExpression[] > keyValuePair in ifExpression.ConditionMap )
                    {
                        ifInfo.SubBranches.Add( CheckReturnValues( keyValuePair.Value, exceptions ) );
                    }

                    info.SubBranches.Add( ifInfo );

                    if ( ifExpression.ElseBlock != null )
                    {
                        info.SubBranches.Add( CheckReturnValues( ifExpression.ElseBlock, exceptions ) );
                    }
                    else
                    {
                        info.SubBranches.Add( new ReturnInfo( false ) );
                    }
                }
                else if ( bsExpression is BSFunctionDefinitionExpression fdef )
                {
                    Validate( fdef, fdef.Block, exceptions );
                }
                else if ( bsExpression is BSEnumerableFunctionDefinitionExpression efdef )
                {
                    Validate( efdef, efdef.Block, exceptions );
                }
            }

            return info;
        }

        private void Validate( BSExpression root, BSExpression[] expressions, List < BSValidatorException > exceptions )
        {
            ReturnInfo info = CheckReturnValues( expressions, exceptions );

            if ( !info.Validate() )
            {
                exceptions.Add(
                               new BSValidatorException(
                                                        $"Function '{root}' needs to have a return statement at every exiting branch or no return statements at all.",
                                                        expressions
                                                       )
                              );
            }
        }

        #endregion

    }

}
