using BadScript.Tools.CodeGenerator.Runtime.Attributes;

namespace BadScript.Tools.CodeGenerator
{

    public class WrapperTypeInfo
    {

        public string Source;
        public string StaticSource;
        public string GeneratedClass;
        public string StaticGeneratedClass;
        public BSWConstructorCreatorAttribute[] Creators;

        #region Public

        public WrapperTypeInfo(
            string src,
            string staticSrc,
            string generatedClass,
            string staticGeneratedClass,
            BSWConstructorCreatorAttribute[] creators = null )
        {
            Source = src;
            StaticSource = staticSrc;
            GeneratedClass = generatedClass;
            StaticGeneratedClass = staticGeneratedClass;
            Creators = creators ?? new BSWConstructorCreatorAttribute[0];
        }

        public string GetWrapperCode( string expr )
        {
            if ( GeneratedClass.StartsWith( "#" ) )
            {
                string[] p = GeneratedClass.Split( ';' );
                string ct = p[0].Remove( 0, 1 );
                string wt = p[1];

                return $"new {wt}(({ct}){expr})";
            }
            else if ( GeneratedClass.StartsWith( "@" ) )
            {
                string[] p = GeneratedClass.Split( ';' );
                string exprF = p[0].Remove( 0, 1 );

                return string.Format( exprF, expr );
            }
            else
            {
                string str = $"new {GeneratedClass}({expr})";

                return str;
            }
        }

        #endregion

    }

}
