using System.Collections.Generic;
using BadScript.Tools.CodeGenerator.Runtime.Attributes;

namespace BadScript.Tools.CodeGenerator
{

    public class WrapperTypeInfo
    {
        public string Source;
        public string GeneratedClass;
        public BSWConstructorCreatorAttribute[] Creators;

        public string GetWrapperCode(string expr)
        {
            if (GeneratedClass.StartsWith("#"))
            {
                string[] p = GeneratedClass.Split(';');
                string ct = p[0].Remove(0, 1);
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

        public WrapperTypeInfo(string src, string generatedClass, BSWConstructorCreatorAttribute[] creators = null )
        {
            Source = src;
            GeneratedClass = generatedClass;
            Creators = creators ?? new BSWConstructorCreatorAttribute[0];
        }

        
    }

}
