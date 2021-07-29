using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Utils
{

    internal class BSReflectionTypeObject : BSObject
    {
        private readonly Dictionary < string, ABSReference > m_Properties;
        private readonly Type m_ReflectedType;

        #region Public

        public BSReflectionTypeObject( Type t, object instance, Dictionary < string, ABSReference > properties ) : base(
            instance )
        {
            m_Properties = properties;
            m_ReflectedType = t;
        }

        public override ABSReference GetProperty( string propertyName )
        {
            if ( m_Properties.ContainsKey( propertyName ) )
            {
                return m_Properties[propertyName];
            }

            return base.GetProperty( propertyName );
        }

        public override bool HasProperty( string propertyName )
        {
            if ( m_Properties.ContainsKey( propertyName ) )
            {
                return true;
            }

            return base.HasProperty( propertyName );
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
        {
            if ( doneList.ContainsKey( this ) )
            {
                return "<recursion>";
            }

            doneList[this] = "{}";

            StringWriter sw = new StringWriter();
            IndentedTextWriter tw = new IndentedTextWriter( sw );
            tw.WriteLine( '{' );
            tw.Indent = 1;
            foreach ( KeyValuePair < string, ABSReference > absReference in m_Properties )
            {
                tw.WriteLine( $"{absReference.Key}" );
            }
            
            tw.Indent = 0;
            tw.WriteLine( '}' );

            doneList[this] = sw.ToString();

            return doneList[this];
        }

        public override void SetProperty( string propertyName, ABSObject obj )
        {
            if ( m_Properties.ContainsKey( propertyName ) )
            {
                m_Properties[propertyName].Assign( obj );
            }

            base.SetProperty( propertyName, obj );
        }

        public override string ToString()
        {
            return m_InternalObject?.ToString() ?? $"STATIC:{m_ReflectedType.FullName}";
        }

        #endregion
    }

}
