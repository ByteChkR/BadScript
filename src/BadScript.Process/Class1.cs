using System;
using System.Collections.Generic;

using BadScript.Runtime;
using BadScript.Runtime.Implementations;

namespace BadScript.Process
{
    public static class ProcessApi
    {
        
        public static void AddApi()
        {
            BSEngine.AddStatic(
                               "start",
                               new BSRuntimeFunction( "function start(file, args)", StartProcess )
                              );
        }

        private static BSRuntimeObject StartProcess( BSRuntimeObject[] args )
        {
            EngineRuntimeTable t = new EngineRuntimeTable();
            string start = args[0].ConvertString();
            string procArgs = args[1].ConvertString();
            System.Diagnostics.Process p  =System.Diagnostics.Process.Start(start, procArgs);


            t.InsertElement(new EngineRuntimeObject("hasExited"), new BSRuntimeFunction(
                                 "function hasExited()",
                                 x => ProcHasExited(p)
                                ));
            t.InsertElement(new EngineRuntimeObject("exitCode"), new BSRuntimeFunction(
                                 "function exitCode()",
                                 x => ProcExitCode(p)
                                ));
            t.InsertElement(new EngineRuntimeObject("abort"), new BSRuntimeFunction(
                                 "function abort()",
                                 x => ProcAbort(p)
                                ));

            t.InsertElement(
                            new EngineRuntimeObject( "waitForExit" ),
                            new BSRuntimeFunction(
                                                  "function waitForExit()/function waitForExit(timeMS)",
                                                  x => ProcWaitForExit( p, x.Length != 0 ? x[0] : null )
                                                 )
                           );

            return t;
        }

        private static BSRuntimeObject ProcHasExited(System.Diagnostics.Process p)
        {
            return new EngineRuntimeObject((decimal)(p.HasExited ? 1 : 0));
        }
        private static BSRuntimeObject ProcExitCode(System.Diagnostics.Process p)
        {
            return new EngineRuntimeObject((decimal)p.ExitCode);
        }
        private static BSRuntimeObject ProcAbort(System.Diagnostics.Process p)
        {
            p.Kill();
            return new EngineRuntimeObject(null);
        }

        private static BSRuntimeObject ProcWaitForExit(System.Diagnostics.Process p, BSRuntimeObject time=null)
        {
            if ( time == null )
                p.WaitForExit();
            else
                p.WaitForExit( ( int ) time.ConvertDecimal() );
            return new EngineRuntimeObject(null);
        }

    }
}
