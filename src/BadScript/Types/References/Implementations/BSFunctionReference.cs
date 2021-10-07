using BadScript.Exceptions;

namespace BadScript.Types.References.Implementations
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

        #region Protected

        protected override int GetHashCodeImpl()
        {
            return m_Func.GetHashCode() ^ 397;
        }

        #endregion

    }

}
