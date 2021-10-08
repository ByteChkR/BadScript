using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

using BadScript.Exceptions;
using BadScript.Parser.Expressions;
using BadScript.Reflection;
using BadScript.Types.Implementations;
using BadScript.Types.References;
using BadScript.Types.References.Implementations;

namespace BadScript.Types
{

    /// <summary>
    ///     Implements a BS Function
    /// </summary>
    public class BSFunction : ABSObject
    {

        protected readonly string DebugData = null;

        private class BSCachedFunction
        {

            private ABSReference m_Reference;
            private readonly Func < ABSReference > m_Creator;

            public ABSReference Reference => m_Reference ??= m_Creator();

            #region Public

            public BSCachedFunction( Func < ABSReference > fnc )
            {
                m_Creator = fnc;
            }

            #endregion

        }

        private static readonly Dictionary < Thread, Stack < BSFunction > > s_Stacks =
            new Dictionary < Thread, Stack < BSFunction > >();

        private readonly (int min, int max) m_ParameterCount;

        private readonly List < BSFunction > m_Hooks = new List < BSFunction >();
        private readonly Dictionary<string, BSCachedFunction> m_CachedFunctions;
        private readonly Dictionary<string, ABSReference> m_Properties = new Dictionary < string, ABSReference >();

        private Func < ABSObject[],
            ABSObject > m_Func;

        public int MinParameters => m_ParameterCount.min;

        public int MaxParameters => m_ParameterCount.max;

        /// <summary>
        ///     The Stacktrace of the Current Thread
        /// </summary>
        public static string[] StackTrace
        {
            get
            {
                if ( !s_Stacks.ContainsKey( Thread.CurrentThread ) )
                {
                    return Array.Empty < string >();
                }

                return s_Stacks[Thread.CurrentThread].Select( x => x.DebugData ).ToArray();
            }
        }

        /// <summary>
        ///     The flattened Stacktrace of the Current Thread
        /// </summary>
        public static string FlatTrace
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach ( string s in StackTrace )
                {
                    sb.AppendLine( s );
                }

                return sb.ToString();
            }
        }

        /// <summary>
        ///     The Stack Depth of the Current thread
        /// </summary>
        private static int StackCount => s_Stacks.Count == 0 ? 0 : s_Stacks[Thread.CurrentThread].Count;

        #region Public

        /// <summary>
        ///     Creates a new BSFunction Instance
        /// </summary>
        /// <param name="debugData">Debug Data</param>
        /// <param name="func">Function Implementation</param>
        /// <param name="argCount">Argument Count</param>
        public BSFunction(
            string debugData,
            Func < ABSObject[], ABSObject >
                func,
            int argCount ) : this( SourcePosition.Unknown, debugData, func, argCount )
        {
        }

        /// <summary>
        ///     Creates a new BSFunction Instance
        /// </summary>
        /// <param name="pos">Source Position</param>
        /// <param name="debugData">Debug Data</param>
        /// <param name="func">Function Implementation</param>
        /// <param name="argCount">Argument Count</param>
        public BSFunction(
            SourcePosition pos,
            string debugData,
            Func < ABSObject[], ABSObject >
                func,
            int argCount ) : this( pos, debugData, func )
        {
            m_ParameterCount = ( argCount, argCount );
        }

        /// <summary>
        ///     Creates a new BSFunction Instance
        /// </summary>
        /// <param name="pos">Source Position</param>
        /// <param name="debugData">Debug Data</param>
        /// <param name="func">Function Implementation</param>
        /// <param name="minArgs">The Minimum amount of arguments</param>
        /// <param name="maxArgs">The Maximum amount of arguments</param>
        public BSFunction(
            SourcePosition pos,
            string debugData,
            Func < ABSObject[], ABSObject >
                func,
            int minArgs,
            int maxArgs ) : this( pos, debugData, func )
        {
            m_ParameterCount = ( minArgs, maxArgs );
        }

        /// <summary>
        ///     Creates a new BSFunction Instance
        /// </summary>
        /// <param name="debugData">Debug Data</param>
        /// <param name="func">Function Implementation</param>
        /// <param name="minArgs">The Minimum amount of arguments</param>
        /// <param name="maxArgs">The Maximum amount of arguments</param>
        public BSFunction(
            string debugData,
            Func < ABSObject[], ABSObject >
                func,
            int minArgs,
            int maxArgs ) : this( SourcePosition.Unknown, debugData, func, minArgs, maxArgs )
        {
            m_ParameterCount = ( minArgs, maxArgs );
        }

        /// <summary>
        ///     Returns the Top Most function on the Call stack
        /// </summary>
        /// <returns></returns>
        public static BSFunction GetTopStack()
        {
            return StackCount == 0 ? null : PeekStack();
        }

        /// <summary>
        ///     Unwind the stack until the stack top is equal to the specified function
        /// </summary>
        /// <param name="top">new Stack Top</param>
        public static void RestoreStack( BSFunction top )
        {
            if ( top == null )
            {
                return;
            }

            while ( PeekStack() != top )
            {
                PopStack();
            }
        }

        /// <summary>
        ///     Add a function hook to this function
        /// </summary>
        /// <param name="func"></param>
        public void AddHook( BSFunction func )
        {
            m_Hooks.Add( func );
        }

        /// <summary>
        ///     Clear all function hooks
        /// </summary>
        public void ClearHooks()
        {
            m_Hooks.Clear();
        }

        public override bool Equals( ABSObject other )
        {
            return ReferenceEquals( this, other );
        }

        public override ABSReference GetProperty( string propertyName )
        {
            
            if ( m_CachedFunctions.ContainsKey( propertyName ) )
            {
                return m_CachedFunctions[propertyName].Reference;
            }

            if ( m_Properties.ContainsKey( propertyName ) )
                return m_Properties[propertyName];

            throw new BSRuntimeException( Position, $"Property {propertyName} does not exist" );
        }

        public override bool HasProperty( string propertyName )
        {
            return m_CachedFunctions.ContainsKey( propertyName ) || m_Properties.ContainsKey(propertyName);
        }

        /// <summary>
        ///     Invokes the current object with the specified arguments
        /// </summary>
        /// <param name="args">Arguments for the invocation</param>
        /// <param name="executeHooks">if true, the registered hooks get executed.</param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject Invoke( ABSObject[] args, bool executeHooks )
        {
            PushStack( this );

            ABSObject[] arr = null;

            if ( executeHooks )
            {
                if ( m_Hooks.Count != 0 )
                {
                    arr = new ABSObject[] { this, new BSArray( args.Select( x => x.ResolveReference() ) ) };
                }

                foreach ( BSFunction bsFunction in m_Hooks )
                {
                    ABSObject o = bsFunction.Invoke( arr );

                    if ( !o.IsNull() )
                    {
                        PopStack();

                        return o;
                    }
                }
            }

            ( int min, int max ) = m_ParameterCount;

            if ( args.Length < min || args.Length > max )
            {
                throw new BSRuntimeException(
                                             Position,
                                             $"Invalid parameter Count: '{DebugData}' expected {min} - {max} and got {args.Length}"
                                            );
            }

            ABSObject or = m_Func( args );
            PopStack();

            return or;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public override ABSObject Invoke( ABSObject[] args )
        {
            return Invoke( args, true );
        }

        public override bool IsNull()
        {
            return false;
        }

        /// <summary>
        ///     Remove a function hook from this function
        /// </summary>
        /// <param name="func"></param>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void RemoveHook( BSFunction func )
        {
            m_Hooks.Remove( func );
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
        {
            return DebugData ?? m_Func.ToString();
        }

        public override void SetProperty( string propertyName, ABSObject obj )
        {
            throw new BSRuntimeException( Position, $"Property {propertyName} does not exist" );
        }

        public override bool TryConvertBool( out bool v )
        {
            v = false;

            return false;
        }

        public override bool TryConvertDecimal( out decimal d )
        {
            d = decimal.Zero;

            return false;
        }

        public override bool TryConvertString( out string v )
        {
            v = null;

            return false;
        }

        #endregion

        #region Protected

        protected override int GetHashCodeImpl()
        {
            return m_Func?.GetHashCode() ?? 0 ^ m_Properties.GetHashCode() ^ m_CachedFunctions.GetHashCode();
        }

        protected void SetFunc( Func < ABSObject[], ABSObject > func )
        {
            m_Func = func;
        }

        #endregion

        #region Private

        private BSFunction(
            SourcePosition pos,
            string debugData,
            Func < ABSObject[], ABSObject >
                func ) : base( pos )
        {
            DebugData = debugData;
            m_Func = func;
            m_CachedFunctions = new();

            m_Properties["MaxArgs"] =
                new BSReflectionReference(() => new BSObject((decimal)m_ParameterCount.max), null);
            m_Properties["MinArgs"] =
                new BSReflectionReference(() => new BSObject((decimal)m_ParameterCount.min), null);

            m_CachedFunctions["Invoke"] = new BSCachedFunction(
                                                          () => new BSFunctionReference(
                                                               new BSFunction(
                                                                              "function Invoke(args)/Invoke(args, execHooks)",
                                                                              x =>
                                                                              {
                                                                                  if ( x.Length == 1 )
                                                                                  {
                                                                                      return Invoke(
                                                                                           ( x[0].ResolveReference() as
                                                                                                   BSArray ).
                                                                                           Elements
                                                                                          );
                                                                                  }

                                                                                  return Invoke(
                                                                                       ( x[0].ResolveReference() as
                                                                                               BSArray ).Elements,
                                                                                       x[1].ConvertBool()
                                                                                      );
                                                                              },
                                                                              1,
                                                                              2
                                                                             )
                                                              )
                                                         );

            m_CachedFunctions["Hook"] =
                new BSCachedFunction(
                                     () => new BSFunctionReference(
                                                                   new BSFunction(
                                                                        "function Hook(hookFunc)",
                                                                        HookFunction,
                                                                        1
                                                                       )
                                                                  )
                                    );

            m_CachedFunctions["ReleaseHook"] =
                new BSCachedFunction(
                                     () => new BSFunctionReference(
                                                                   new BSFunction(
                                                                        "function ReleaseHook(hookFunc)",
                                                                        ReleaseHookFunction,
                                                                        1
                                                                       )
                                                                  )
                                    );

            m_CachedFunctions["ReleaseHooks"] = new BSCachedFunction(
                                                                () => new BSFunctionReference(
                                                                     new BSFunction(
                                                                          "function ReleaseHook()",
                                                                          ReleaseHooksFunction,
                                                                          0
                                                                         )
                                                                    )
                                                               );
        }

        private static BSFunction PeekStack()
        {
            if ( s_Stacks.ContainsKey( Thread.CurrentThread ) )
            {
                return s_Stacks[Thread.CurrentThread].Peek();
            }

            throw new BSRuntimeException( "Can not Peek a function from an empty stack." );
        }

        private static BSFunction PopStack()
        {
            if ( s_Stacks.ContainsKey( Thread.CurrentThread ) )
            {
                return s_Stacks[Thread.CurrentThread].Pop();
            }

            throw new BSRuntimeException( "Can not Pop a function off an empty stack." );
        }

        private static void PushStack( BSFunction f )
        {
            if ( s_Stacks.ContainsKey( Thread.CurrentThread ) )
            {
                s_Stacks[Thread.CurrentThread].Push( f );
            }
            else
            {
                s_Stacks[Thread.CurrentThread] = new Stack < BSFunction >( new[] { f } );
            }
        }

        private ABSObject HookFunction( ABSObject[] arg )
        {
            if ( arg[0].ResolveReference() is BSFunction hook )
            {
                AddHook( hook );

                return BSObject.Null;
            }

            throw new BSInvalidTypeException(
                                             SourcePosition.Unknown,
                                             "Expected Function as argument.",
                                             arg[0],
                                             "BSFunction"
                                            );
        }

        private ABSObject ReleaseHookFunction( ABSObject[] arg )
        {
            if ( arg[0].ResolveReference() is BSFunction hook )
            {
                RemoveHook( hook );

                return BSObject.Null;
            }

            throw new BSInvalidTypeException(
                                             SourcePosition.Unknown,
                                             "Expected Function as argument.",
                                             arg[0],
                                             "BSFunction"
                                            );
        }

        private ABSObject ReleaseHooksFunction( ABSObject[] arg )
        {
            ClearHooks();

            return BSObject.Null;
        }

        #endregion

    }

}
