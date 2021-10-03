using BadScript.Exceptions;
using BadScript.Types.References;

namespace BadScript.Types.Implementations
{

    public class BSFunctionReference : ABSReference
    {

        private readonly BSFunction m_Func;

        #region Public

        public BSFunctionReference( BSFunction f ) : base( f.Position )
        {
            m_Func = f;
        }

        public override void Assign( ABSObject obj )
        {
            throw new BSRuntimeException( Position, "Can not Assign Function in Array" );
        }

        public override ABSObject Get()
        {
            return m_Func;
        }

        #endregion

    }

}
