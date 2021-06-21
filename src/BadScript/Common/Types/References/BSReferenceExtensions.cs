namespace BadScript.Common.Types.References
{

    public static class BSReferenceExtensions
    {

        #region Public

        public static ABSObject ResolveReference( this ABSObject o )
        {
            while ( o is ABSReference r )
            {
                o = r.Get();
            }

            return o;
        }

        #endregion

    }

}
