using System.Collections.Generic;

using BadScript.Common.Expressions;

namespace BadScript.Common.Types.References
{
    /// <summary>
    /// Class used to Assign Values to variables
    /// </summary>
    public abstract class ABSReference : ABSObject
    {

        public override bool IsNull => Get().IsNull;

        #region Public

        /// <summary>
        /// Assigns a value to the element that this reference is pointing to
        /// </summary>
        /// <param name="obj">Object to Assign</param>
        public abstract void Assign( ABSObject obj );

        /// <summary>
        /// Returns the Value of the Reference
        /// </summary>
        /// <returns></returns>
        public abstract ABSObject Get();

        public override bool Equals( ABSObject other )
        {
            return Get().Equals( other.ResolveReference() );
        }

        public override ABSReference GetProperty( string propertyName )
        {
            return Get().GetProperty( propertyName );
        }

        public override bool HasProperty( string propertyName )
        {
            return Get().HasProperty( propertyName );
        }

        public override ABSObject Invoke( ABSObject[] args )
        {
            return Get().Invoke( args );
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
        {
            return Get().SafeToString( doneList );
        }

        public override void SetProperty( string propertyName, ABSObject obj )
        {
            Get().SetProperty( propertyName, obj );
        }

        public override string ToString()
        {
            return Get().ToString();
        }

        public override bool TryConvertBool( out bool v )
        {
            return Get().TryConvertBool( out v );
        }

        public override bool TryConvertDecimal( out decimal d )
        {
            return Get().TryConvertDecimal( out d );
        }

        public override bool TryConvertString( out string v )
        {
            return Get().TryConvertString( out v );
        }

        #endregion

        #region Protected

        protected ABSReference( SourcePosition pos ) : base( pos )
        {
        }

        #endregion

    }

}
