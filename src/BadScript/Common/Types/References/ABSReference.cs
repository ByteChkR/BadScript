namespace BadScript.Common.Types.References
{

    public abstract class ABSReference : ABSObject
    {
        #region Public

        public abstract void Assign( ABSObject obj );

        public abstract ABSObject Get();

        #endregion
    }

}
