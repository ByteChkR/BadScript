namespace BadScript.Runtime
{

    public abstract class BSRuntimeTable : BSRuntimeObject
    {

        public abstract BSRuntimeArray Keys { get; }

        public abstract BSRuntimeArray Values { get; }

        #region Public

        public abstract void Clear();

        public abstract BSRuntimeReference GetElement( BSRuntimeObject i );

        public abstract int GetLength();

        public abstract BSRuntimeObject GetRawElement( BSRuntimeObject k );

        public abstract bool HasElement( BSRuntimeObject i );

        public abstract void InsertElement( BSRuntimeObject k, BSRuntimeObject o );

        public abstract void RemoveElement( BSRuntimeObject k );

        #endregion

    }

}
