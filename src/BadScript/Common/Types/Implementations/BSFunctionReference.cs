using BadScript.Common.Exceptions;
using BadScript.Common.Types.References;

namespace BadScript.Common.Types.Implementations
{

    public class BSFunctionReference : ABSReference
    {
        private readonly BSFunction m_Func;

        #region Public

        public BSFunctionReference( BSFunction f )
        {
            m_Func = f;
        }

        public override void Assign( ABSObject obj )
        {
            throw new BSRuntimeException( "Can not Assign Function in Array" );
        }

        public override ABSObject Get()
        {
            return m_Func;
        }

        #endregion
    }

}