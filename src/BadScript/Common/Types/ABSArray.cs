using BadScript.Common.Types.References;

namespace BadScript.Common.Types
{

    public abstract class ABSArray : ABSObject
    {
        #region Public

        public abstract ABSReference GetElement( int i );

        public abstract int GetLength();

        public abstract ABSObject GetRawElement( int i );

        public abstract void InsertElement( int i, ABSObject o );

        public abstract void RemoveElement( int i );

        public abstract void SetElement( int k, ABSObject o );

        public void AddElement( ABSObject o )
        {
            InsertElement( GetLength(), o );
        }

        public virtual void Clear()
        {
            while ( GetLength() != 0 )
            {
                RemoveElement( 0 );
            }
        }

        #endregion
    }

}
