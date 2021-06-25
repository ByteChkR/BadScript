using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BadScript.ExpGen
{

    internal class Program
    {
        private static Process m_Evaluator;

        private static readonly Random s_Rnd = new Random();

        #region Public

        public static string Generate( int exprs = 10 )
        {
            string xS = GenerateTerminal();

            for ( int j = 0; j < exprs; j++ )
            {
                string op = GenerateOperator();
                string yS = GenerateTerminal();
                xS = xS + op + yS;
            }

            return xS;

        }

        public static string GenerateOperator()
        {
            int i = s_Rnd.Next( 0, 3 );

            if ( i == 0 )
            {
                return " * ";
            }

            if ( i == 1 )
            {
                return " - ";
            }

            return " + ";
        }

        public static string GenerateTerminal()
        {
            double d = s_Rnd.Next( 1, 100 );

            return d.ToString();
        }

        #endregion

        #region Private

        private static string Evaluate( string expr )
        {
            m_Evaluator.StandardInput.WriteLine( expr );
            string l = m_Evaluator.StandardOutput.ReadLine();

            while ( l.StartsWith( "PS" ) )
            {
                l = m_Evaluator.StandardOutput.ReadLine();
            }

            return l;
        }

        private static string GenerateCode( int exprNum, string expr )
        {
            string val = Evaluate( expr );
            Console.WriteLine( expr + " = " + val );
            string exprResult = $"exprResult_{exprNum}";
            StringBuilder sb = new StringBuilder( $"{exprResult} = " );
            sb.AppendLine( expr );
            sb.AppendLine( $"if({exprResult} != {val})" );
            sb.AppendLine( "{" );

            sb.AppendLine(
                $"\tprint(\"\t[FAILED] Expression: {exprResult}\\n\tExpected: {val}\\n\tGot: \" + {exprResult})" );

            sb.AppendLine( "}" );
            sb.AppendLine( "else" );
            sb.AppendLine( "{" );
            sb.AppendLine( $"\tprint(\"\t[Passed] Expression: {exprResult}\")" );
            sb.AppendLine( "}" );

            return sb.ToString();
        }

        private static void Initialize()
        {
            ProcessStartInfo psi = new ProcessStartInfo( "powershell.exe", "-NoLogo" );
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            m_Evaluator = new Process();
            m_Evaluator.StartInfo = psi;
            m_Evaluator.Start();
        }

        private static void Main( string[] args )
        {
            Initialize();
            StringBuilder sb = new StringBuilder();
            int exprs = 100;
            int exprMinLength = 2;
            int exprMaxLength = 3;

            if ( args.Length > 0 && int.TryParse( args[0], out int expressionCount ) )
            {
                exprs = expressionCount;
            }

            if ( args.Length > 1 && int.TryParse( args[1], out int expressionMin ) )
            {
                exprMinLength = expressionMin;
            }

            if ( args.Length > 2 && int.TryParse( args[2], out int expressionMax ) )
            {
                exprMaxLength = expressionMax;
            }

            for ( int i = 0; i < exprs; i++ )
            {
                int l = s_Rnd.Next( exprMinLength, exprMaxLength );
                string expr = Generate( l );
                sb.AppendLine( GenerateCode( i, expr ) );
            }

            File.WriteAllText( "./precedence_tests.bs", sb.ToString() );
            m_Evaluator.StandardInput.WriteLine( "exit" );
            m_Evaluator.WaitForExit();
        }

        #endregion
    }

}
