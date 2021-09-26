using System.Collections.Generic;
using System.Linq;

using BadScript.Console.Subsystems.Project.Utils;

namespace BadScript.Console.Subsystems.Project.DataObjects
{

    public class ProjectBuildTargetCollection : ReflectedObject
    {

        public List < BuildTarget > Targets;

        #region Public

        public ProjectBuildTargetCollection() : this( null )
        {
        }

        public ProjectBuildTargetCollection( IEnumerable < BuildTarget > targets )
        {
            if ( targets != null )
            {
                Targets = new(targets);
            }
            else
            {
                Targets = new();
            }
        }

        public override object GetProperty( string name, ReflectionResolveInfo info )
        {
            BuildTarget t = GetTarget( name );

            if ( t != null )
            {
                return t;
            }

            return base.GetProperty( name, info );
        }

        public BuildTarget GetTarget( string name )
        {
            return Targets.FirstOrDefault( x => x.Name == name );
        }

        #endregion

    }

}
