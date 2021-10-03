using BadScript.Interfaces;
using BadScript.Parser.Expressions;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Process
{

    public class BSProcessInterface : ABSScriptInterface
    {

        #region Public

        public BSProcessInterface() : base( "process" )
        {
        }

        public override void AddApi( ABSTable proc )
        {
            proc.InsertElement(
                               new BSObject( "start" ),
                               new BSFunction( "function start(file, args)", StartProcess, 2 )
                              );
        }

        #endregion

        #region Private

        private static ABSObject ProcAbort( System.Diagnostics.Process p )
        {
            p.Kill();

            return BSObject.Null;
        }

        private static ABSObject ProcExitCode( System.Diagnostics.Process p )
        {
            return new BSObject( ( decimal )p.ExitCode );
        }

        private static ABSObject ProcHasExited( System.Diagnostics.Process p )
        {
            return p.HasExited ? BSObject.True : BSObject.False;
        }

        private static ABSObject ProcWaitForExit( System.Diagnostics.Process p, ABSObject time = null )
        {
            if ( time == null )
            {
                p.WaitForExit();
            }
            else
            {
                p.WaitForExit( ( int )time.ResolveReference().ConvertDecimal() );
            }

            return BSObject.Null;
        }

        private static ABSObject StartProcess( ABSObject[] args )
        {
            BSTable t = new BSTable( SourcePosition.Unknown );
            string start = args[0].ResolveReference().ConvertString();
            string procArgs = args[1].ResolveReference().ConvertString();
            System.Diagnostics.Process p = System.Diagnostics.Process.Start( start, procArgs );

            t.InsertElement(
                            new BSObject( "hasExited" ),
                            new BSFunction(
                                           "function hasExited()",
                                           x => ProcHasExited( p ),
                                           0
                                          )
                           );

            t.InsertElement(
                            new BSObject( "exitCode" ),
                            new BSFunction(
                                           "function exitCode()",
                                           x => ProcExitCode( p ),
                                           0
                                          )
                           );

            t.InsertElement(
                            new BSObject( "abort" ),
                            new BSFunction(
                                           "function abort()",
                                           x => ProcAbort( p ),
                                           0
                                          )
                           );

            t.InsertElement(
                            new BSObject( "waitForExit" ),
                            new BSFunction(
                                           "function waitForExit()/function waitForExit(timeMS)",
                                           x => ProcWaitForExit( p, x.Length != 0 ? x[0] : null ),
                                           0,
                                           1
                                          )
                           );

            return t;
        }

        #endregion

    }

}
