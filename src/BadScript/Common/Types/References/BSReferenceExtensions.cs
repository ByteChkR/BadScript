using System.Runtime.CompilerServices;

namespace BadScript.Common.Types.References
{

    public static class BSReferenceExtensions
    {
        #region Public

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
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
