using System;
using System.Diagnostics;
using System.Text;

namespace BadScript.Console.Subsystems.Project.Utils
{

    public class ProcessEventRunner
    {

        public enum ProcessExitMode
        {

            Success,
            Failed

        }

        public ProcessStartInfo StartInfo;
        private ProcessExitMode m_Mode = ProcessExitMode.Success;
        private readonly string m_ProcName;
        private readonly bool m_WriteOutput;
        private readonly StringBuilder m_ConsoleError;
        private readonly StringBuilder m_ConsoleOutput;

        public string ErrorData => m_ConsoleError.ToString();

        public string OutputData => m_ConsoleOutput.ToString();

        public int IndentationCount { get; set; }

        private string Indentation
        {
            get
            {
                string s = "";

                for ( int i = 0; i < IndentationCount; i++ )
                {
                    s += "\t";
                }

                return s;
            }
        }

        #region Public

        public ProcessEventRunner(
            string procName,
            string cmd,
            bool writeOutput = true,
            int indentation = 2 )
        {
            IndentationCount = indentation;
            m_ProcName = procName;
            m_ConsoleError = new StringBuilder();
            m_ConsoleOutput = new StringBuilder();
            m_WriteOutput = writeOutput;
            string exec = cmd;
            string args;
            int split = cmd.IndexOf( ' ' );

            if ( split == -1 )
            {
                args = "";
            }
            else
            {
                exec = cmd.Remove( split );
                args = cmd.Substring( split + 1 );
            }

            StartInfo = new ProcessStartInfo( exec, args );
            StartInfo.UseShellExecute = false;
            StartInfo.CreateNoWindow = true;
            StartInfo.RedirectStandardError = true;
            StartInfo.RedirectStandardOutput = true;
            StartInfo.RedirectStandardInput = true;
        }

        public ProcessExitMode Run()
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = StartInfo;
            p.EnableRaisingEvents = true;

            p.ErrorDataReceived += ( sender, args ) =>
                                   {
                                       if ( !string.IsNullOrEmpty( args.Data ) )
                                       {
                                           if ( m_WriteOutput )
                                           {
                                               ConsoleWriter.ErrorLine(
                                                                       $"{Indentation}[{m_ProcName}][ERROR] " +
                                                                       args.Data
                                                                      );
                                           }

                                           m_ConsoleError.AppendLine( args.Data );
                                           m_Mode = ProcessExitMode.Failed;
                                       }
                                   };

            p.OutputDataReceived += ( sender, args ) =>
                                    {
                                        if ( !string.IsNullOrEmpty( args.Data ) )
                                        {
                                            if ( m_WriteOutput )
                                            {
                                                ConsoleWriter.LogLine(
                                                                      $"{Indentation}[{m_ProcName}][LOG]" + args.Data
                                                                     );
                                            }

                                            m_ConsoleOutput.AppendLine( args.Data );
                                        }
                                    };

            p.Start();
            p.BeginErrorReadLine();
            p.BeginOutputReadLine();

            System.Console.TreatControlCAsInput = true;
            ConsoleWriter.LogLine( $"{Indentation}[PR] Input is now Redirected for process '{m_ProcName}'" );

            while ( !p.HasExited )
            {
                if ( System.Console.KeyAvailable )
                {
                    ConsoleKeyInfo info = System.Console.ReadKey();

                    p.StandardInput.Write( info.KeyChar );
                }
            }

            System.Console.TreatControlCAsInput = false;

            return m_Mode;
        }

        #endregion

    }

}
