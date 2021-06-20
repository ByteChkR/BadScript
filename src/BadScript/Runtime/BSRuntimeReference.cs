namespace BadScript.Runtime
{

    public abstract class BSRuntimeReference : BSRuntimeObject
    {

        #region Public

        public abstract void Assign( BSRuntimeObject obj );

        public abstract BSRuntimeObject Get();

        #endregion

    }

}
