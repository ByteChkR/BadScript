using BadScript.Exceptions;
using BadScript.Parser.Expressions;
using BadScript.Types;
using BadScript.Types.References;

namespace BadScript.Parser.OperatorImplementations.Implementations
{

    public class BSArrayAccessOperatorImplementation : ABSOperatorImplementation
    {

        #region Public

        public BSArrayAccessOperatorImplementation() : base( "[]" )
        {
        }

        public override bool IsCorrectImplementation( ABSObject[] args )
        {
            return true;
        }

        #endregion

        #region Protected

        protected override ABSObject Execute( ABSObject[] args )
        {
            ABSObject obj = args[0].ResolveReference();
            ABSObject i = args[1];

            if ( obj is ABSTable t )
            {
                return t.GetElement( i );
            }

            if ( obj is ABSArray a )
            {
                if ( i.TryConvertDecimal( out decimal d ) )
                {
                    return a.GetElement( ( int )d );
                }
            }

            throw new BSInvalidTypeException(
                                             SourcePosition.Unknown,
                                             "Expected Array",
                                             obj,
                                             "Table"
                                            );
        }

        #endregion

    }

}
