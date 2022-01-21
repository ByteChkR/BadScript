using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BadScript.Interfaces;
using BadScript.Parser.Expressions;
using BadScript.Reflection;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Threading
{

    public class BSThreadingInterface : ABSScriptInterface
    {

        #region Public

        public BSThreadingInterface() : base( "Threading" )
        {
        }

        public override void AddApi( ABSTable root )
        {
            root.InsertElement(
                               "CurrentThread",
                               new BSReflectionReference( () => CreateThreadObject( Thread.CurrentThread ), null )
                              );

            root.InsertElement(
                               "CreateThread",
                               new BSFunction( "function CreateThread(fnc, args..)", CreateThread, 1, int.MaxValue )
                              );

            root.InsertElement(
                               "CreateTask",
                               new BSFunction( "function CreateTask(fnc, args..)", CreateTask, 1, int.MaxValue )
                              );
        }

        #endregion

        #region Private

        private static ABSObject AbortThread( Thread t )
        {
            t.Abort();

            return BSObject.Null;
        }

        private static ABSTable CreateTaskObject( Task < ABSObject > task )
        {
            BSTable t = new BSTable( SourcePosition.Unknown );
            t.SetRawElement( "Result", new BSReflectionReference( () => task.Result, null ) );
            t.SetRawElement( "RawTask", new BSObject( task ) );

            t.SetRawElement(
                            "IsCompleted",
                            new BSReflectionReference(
                                                      () =>
                                                          task.IsCompleted ? BSObject.True : BSObject.False,
                                                      null
                                                     )
                           );

            t.SetRawElement(
                            "IsFaulted",
                            new BSReflectionReference( () => task.IsFaulted ? BSObject.True : BSObject.False, null )
                           );

            t.SetRawElement(
                            "IsCanceled",
                            new BSReflectionReference( () => task.IsCanceled ? BSObject.True : BSObject.False, null )
                           );

            t.SetRawElement(
                            "IsCompletedSuccessfully",
                            new BSReflectionReference(
                                                      () =>
                                                          task.Status == TaskStatus.RanToCompletion
                                                              ? BSObject.True
                                                              : BSObject.False,
                                                      null
                                                     )
                           );

            t.SetRawElement(
                            "Status",
                            new BSReflectionReference( () => new BSObject( task.Status.ToString() ), null )
                           );

            t.SetRawElement(
                            "StatusCode",
                            new BSReflectionReference(
                                                      () =>
                                                          new BSObject( ( decimal )task.Status ),
                                                      null
                                                     )
                           );

            t.InsertElement(
                            "Wait",
                            new BSFunction( "function Wait()/Wait(msTimeout)", arg => WaitTask( task, arg ), 0, 1 )
                           );

            t.Lock();

            return t;
        }

        private static ABSTable CreateThreadObject( Thread task )
        {
            BSTable t = new BSTable( SourcePosition.Unknown );
            t.SetRawElement( "RawThread", new BSReflectionReference( () => new BSObject( task ), null ) );

            t.InsertElement(
                            "Name",
                            "UnnamedThread"
                           );

            t.SetRawElement(
                            "IsBackground",
                            new BSReflectionReference(
                                                      () => new BSObject( task.IsBackground ),
                                                      o => task.IsBackground = o.ConvertBool()
                                                     )
                           );

            t.SetRawElement(
                            "ThreadID",
                            new BSReflectionReference(
                                                      () => new BSObject( ( decimal )task.ManagedThreadId ),
                                                      null
                                                     )
                           );

            t.SetRawElement(
                            "IsAlive",
                            new BSReflectionReference(
                                                      () =>
                                                          new BSObject( task.IsAlive ),
                                                      null
                                                     )
                           );

            t.InsertElement( "Abort", new BSFunction( "function Abort()", args => AbortThread( task ), 0 ) );

            t.InsertElement(
                            "Interrupt",
                            new BSFunction( "function Interrupt()", args => InterruptThread( task ), 0 )
                           );

            t.InsertElement(
                            "Join",
                            new BSFunction( "function Join()/Join(msTimeout)", args => JoinThread( task, args ), 0, 1 )
                           );

            t.InsertElement(
                            "Start",
                            new BSFunction(
                                           "function Start()",
                                           args =>
                                               StartThread( task ),
                                           0
                                          )
                           );

            t.Lock();

            return t;
        }

        private static ABSObject InterruptThread( Thread t )
        {
            t.Interrupt();

            return BSObject.Null;
        }

        private static ABSObject JoinThread( Thread t, ABSObject[] args )
        {
            if ( args.Length == 1 )
            {
                t.Join( ( int )args[0].ConvertDecimal() );
            }
            else
            {
                t.Join();
            }

            return BSObject.Null;
        }

        private static ABSObject StartTask( Task < ABSObject > task )
        {
            return BSObject.Null;
        }

        private static ABSObject StartThread( Thread t )
        {
            t.Start();

            return BSObject.Null;
        }

        private static ABSObject WaitTask( Task < ABSObject > task, ABSObject[] arg )
        {
            if ( arg.Length == 1 )
            {
                task.Wait( ( int )arg[0].ConvertDecimal() );
            }
            else
            {
                task.Wait();
            }

            return BSObject.Null;
        }

        private ABSObject CreateTask( ABSObject[] arg )
        {
            BSFunction func = ( BSFunction )arg[0].ResolveReference();
            ABSObject[] args = arg.Skip( 1 ).Select( x => x.ResolveReference() ).ToArray();

            return CreateTaskObject( Task.Run( () => func.Invoke( args ) ) );
        }

        private ABSObject CreateThread( ABSObject[] arg )
        {
            BSFunction func = ( BSFunction )arg[0].ResolveReference();
            ABSObject[] args = arg.Skip( 1 ).Select( x => x.ResolveReference() ).ToArray();

            return CreateThreadObject( new Thread( () => func.Invoke( args ) ) );
        }

        #endregion

    }

}
