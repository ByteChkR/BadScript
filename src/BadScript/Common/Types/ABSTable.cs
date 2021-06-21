using BadScript.Common.Types.References;

namespace BadScript.Common.Types
{

    public abstract class ABSTable : ABSObject
    {

        public abstract ABSArray Keys { get; }

        public abstract ABSArray Values { get; }

        #region Public

        public abstract void Clear();

        public abstract ABSReference GetElement( ABSObject i );

        public abstract int GetLength();

        public abstract ABSObject GetRawElement( ABSObject k );

        public abstract bool HasElement( ABSObject i );

        public abstract void InsertElement( ABSObject k, ABSObject o );

        public abstract void RemoveElement( ABSObject k );

        #endregion

    }

}
