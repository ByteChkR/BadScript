using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Process
{

    public static class ProcessApi
    {

        #region Public

        public static void AddApi()
        {
            BSEngine.AddStatic(
                               "start",
                               new BSFunction( "function start(file, args)", StartProcess )
                              );
        }

        #endregion

        #region Private

        private static ABSObject ProcAbort( System.Diagnostics.Process p )
        {
            p.Kill();

            return new BSObject( null );
        }

        private static ABSObject ProcExitCode( System.Diagnostics.Process p )
        {
            return new BSObject( ( decimal ) p.ExitCode );
        }

        private static ABSObject ProcHasExited( System.Diagnostics.Process p )
        {
            return new BSObject( ( decimal ) ( p.HasExited ? 1 : 0 ) );
        }

        private static ABSObject ProcWaitForExit( System.Diagnostics.Process p, ABSObject time = null )
        {
            if ( time == null )
            {
                p.WaitForExit();
            }
            else
            {
                p.WaitForExit( ( int ) time.ConvertDecimal() );
            }

            return new BSObject( null );
        }

        private static ABSObject StartProcess( ABSObject[] args )
        {
            BSTable t = new BSTable();
            string start = args[0].ConvertString();
            string procArgs = args[1].ConvertString();
            System.Diagnostics.Process p = System.Diagnostics.Process.Start( start, procArgs );

            t.InsertElement(
                            new BSObject( "hasExited" ),
                            new BSFunction(
                                                  "function hasExited()",
                                                  x => ProcHasExited( p )
                                                 )
                           );

            t.InsertElement(
                            new BSObject( "exitCode" ),
                            new BSFunction(
                                                  "function exitCode()",
                                                  x => ProcExitCode( p )
                                                 )
                           );

            t.InsertElement(
                            new BSObject( "abort" ),
                            new BSFunction(
                                                  "function abort()",
                                                  x => ProcAbort( p )
                                                 )
                           );

            t.InsertElement(
                            new BSObject( "waitForExit" ),
                            new BSFunction(
                                                  "function waitForExit()/function waitForExit(timeMS)",
                                                  x => ProcWaitForExit( p, x.Length != 0 ? x[0] : null )
                                                 )
                           );

            return t;
        }

        #endregion

    }

}
