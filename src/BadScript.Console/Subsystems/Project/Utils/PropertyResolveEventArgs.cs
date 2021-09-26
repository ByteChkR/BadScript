namespace BadScript.Console.Subsystems.Project.Utils
{

    public class PropertyResolveEventArgs
    {

        public bool Cancel;
        public readonly string[] Parts;
        public readonly int Current;
        public readonly ReflectionResolveInfo Info;
        public string Result;

        public string Input => Parts[Current];

        #region Public

        public PropertyResolveEventArgs( string[] parts, int current, ReflectionResolveInfo info )
        {
            Parts = parts;
            Current = current;
            Info = info;
        }

        #endregion

    }

}
