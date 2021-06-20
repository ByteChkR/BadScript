namespace BadScript.Runtime
{

    public abstract class BSRuntimeArray : BSRuntimeObject
    {

        #region Public

        public abstract BSRuntimeReference GetElement( int i );

        public abstract int GetLength();

        public abstract BSRuntimeObject GetRawElement( int i );

        public abstract void InsertElement( int i, BSRuntimeObject o );

        public abstract void RemoveElement( int i );

        public abstract void SetElement( int k, BSRuntimeObject o );

        public void AddElement( BSRuntimeObject o )
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
