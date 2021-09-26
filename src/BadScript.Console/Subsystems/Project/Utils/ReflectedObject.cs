using System;
using System.Linq;
using System.Reflection;

namespace BadScript.Console.Subsystems.Project.Utils
{

    public abstract class ReflectedObject
    {

        private readonly FieldInfo[] m_Fields;
        private readonly PropertyInfo[] m_Properties;

        #region Public

        public virtual object GetProperty( string name, ReflectionResolveInfo info )
        {
            FieldInfo fi = m_Fields.FirstOrDefault( x => x.Name == name );

            if ( fi != null )
            {
                return fi.GetValue( this );
            }

            PropertyInfo pi = m_Properties.FirstOrDefault( x => x.Name == name );

            if ( pi != null )
            {
                return pi.GetValue( this );
            }

            throw new Exception( "Can not Find Property: " + name );
        }

        public virtual string ResolveProperty( int current, string[] parts, ReflectionResolveInfo info )
        {
            if ( current >= parts.Length )
            {
                throw new Exception();
            }

            if ( current == parts.Length - 1 )
            {
                return info.Settings.ResolveValue( GetProperty( parts[current], info ).ToString(), info.CurrentTarget );
            }

            object o = GetProperty( parts[current], info );

            if ( o is ReflectedObject so )
            {
                return so.ResolveProperty( current + 1, parts, info );
            }

            throw new Exception( "Can not Resolve Property: " + parts[current] );
        }

        #endregion

        #region Protected

        protected ReflectedObject()
        {
            m_Fields = GetType().GetFields( BindingFlags.Public | BindingFlags.Instance );
            m_Properties = GetType().GetProperties( BindingFlags.Public | BindingFlags.Instance );
        }

        #endregion

    }

}
