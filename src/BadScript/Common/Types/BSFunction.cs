using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Common.Types
{

    public class BSFunction : ABSObject
    {
        private class BSCachedFunction
        {
            private ABSReference m_Reference;
            private readonly Func <ABSReference> m_Creator;
            public ABSReference Reference=> m_Reference ??= m_Creator();
            public BSCachedFunction(Func <ABSReference> fnc)
            {
                m_Creator = fnc;
            }
        }

        private static readonly Dictionary < Thread, Stack < BSFunction > > s_Stacks = new Dictionary < Thread, Stack < BSFunction > >();
        private static void PushStack(BSFunction f)
        {
            if ( s_Stacks.ContainsKey( Thread.CurrentThread ) )
                s_Stacks[Thread.CurrentThread].Push( f );
            else
                s_Stacks[Thread.CurrentThread] = new Stack < BSFunction >( new[] { f } );
        }

        private static int StackCount => s_Stacks[Thread.CurrentThread].Count;
        private static BSFunction PopStack()
        {
            if (s_Stacks.ContainsKey(Thread.CurrentThread))
                return s_Stacks[Thread.CurrentThread].Pop();

            throw new BSRuntimeException("Can not Pop a function off an empty stack.");
        }
        private static BSFunction PeekStack()
        {
            if (s_Stacks.ContainsKey(Thread.CurrentThread))
                return s_Stacks[Thread.CurrentThread].Peek();

            throw new BSRuntimeException("Can not Peek a function from an empty stack.");
        }
        

        private readonly (int min, int max)? m_ParameterCount;
        private readonly string m_DebugData = null;

        private readonly List < BSFunction > m_Hooks = new List < BSFunction >();
        private readonly Dictionary < string, BSCachedFunction> m_Properties;
        private Func < ABSObject[],
            ABSObject > m_Func;

        public static string[] StackTrace
        {
            get
            {
                if ( !s_Stacks.ContainsKey( Thread.CurrentThread ) )
                    return new string[0];
                return s_Stacks[Thread.CurrentThread].Select(x => x.m_DebugData).ToArray();
            }
        }

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

        public override bool IsNull => false;

        #region Public

        public BSFunction(
            string debugData,
            Func < ABSObject[], ABSObject >
                func,
            int argCount ) : this( SourcePosition.Unknown, debugData, func, argCount )
        {
        }

        public BSFunction(
            SourcePosition pos,
            string debugData,
            Func < ABSObject[], ABSObject >
                func,
            int argCount ) : this( pos, debugData, func )
        {
            m_ParameterCount = ( argCount, argCount );
        }

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

        public BSFunction(
            string debugData,
            Func < ABSObject[], ABSObject >
                func,
            int minArgs,
            int maxArgs ) : this( SourcePosition.Unknown, debugData, func, minArgs, maxArgs )
        {
            m_ParameterCount = ( minArgs, maxArgs );
        }

        public static BSFunction GetTopStack()
        {
            return StackCount == 0 ? null : PeekStack();
        }

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

        public void AddHook( BSFunction func )
        {
            m_Hooks.Add( func );
        }

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
            if ( m_Properties.ContainsKey( propertyName ) )
                return m_Properties[propertyName].Reference;
            throw new BSRuntimeException( Position, $"Property {propertyName} does not exist" );
        }

        public override bool HasProperty( string propertyName )
        {
            return m_Properties.ContainsKey(propertyName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ABSObject Invoke(ABSObject[] args, bool executeHooks)
        {
            PushStack(this);

            ABSObject[] arr = null;

            if ( executeHooks )
            {
                if (m_Hooks.Count != 0)
                {
                    arr = new ABSObject[] { this, new BSArray(args.Select(x => x.ResolveReference())) };
                }
                foreach (BSFunction bsFunction in m_Hooks)
                {
                    ABSObject o = bsFunction.Invoke(arr);

                    if (!o.IsNull)
                    {
                        PopStack();

                        return o;
                    }
                }
            }

            if (m_ParameterCount == null)
            {
                ABSObject o = m_Func(args);
                PopStack();

                return o;
            }

            (int min, int max) = m_ParameterCount.Value;

            if (args.Length < min || args.Length > max)
            {
                throw new BSRuntimeException(
                    Position,
                    $"Invalid parameter Count: '{m_DebugData}' expected {min} - {max} and got {args.Length}");
            }

            ABSObject or = m_Func(args);
            PopStack();

            return or;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override ABSObject Invoke( ABSObject[] args )
        {
            return Invoke( args, true );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveHook( BSFunction func )
        {
            m_Hooks.Remove( func );
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
        {
            return m_DebugData ?? m_Func.ToString();
        }

        public override void SetProperty( string propertyName, ABSObject obj )
        {
            throw new BSRuntimeException( Position, $"Property {propertyName} does not exist" );
        }

        public override bool TryConvertBool( out bool v )
        {
            v = true;

            return true;
        }

        public override bool TryConvertDecimal( out decimal d )
        {
            d = 1;

            return false;
        }

        public override bool TryConvertString( out string v )
        {
            v = m_DebugData ?? m_Func.ToString();
            ;

            return true;
        }

        #endregion

        #region Private

        private BSFunction(
            SourcePosition pos,
            string debugData,
            Func < ABSObject[], ABSObject >
                func ) : base( pos )
        {
            m_DebugData = debugData;
            m_Func = func;
            m_Properties = new();

            m_Properties["invoke"] = new BSCachedFunction(
                () => new BSFunctionReference(

                    new BSFunction(
                        "function invoke(args)/invoke(args, execHooks)",
                        x =>
                        {
                            if ( x.Length == 1 )
                                return Invoke( ( x[0].ResolveReference() as BSArray ).Elements );

                            return Invoke(
                                ( x[0].ResolveReference() as BSArray ).Elements,
                                x[1].ConvertBool() );
                        },
                        1,
                        2 )
                )
            );

            m_Properties["hook"] =
                new BSCachedFunction(
                    () => new BSFunctionReference( new BSFunction( "function hook(hookFunc)", HookFunction, 1 ) ) );

            m_Properties["releaseHook"] =
                new BSCachedFunction(
                    () => new BSFunctionReference(
                        new BSFunction( "function releaseHook(hookFunc)", ReleaseHookFunction, 1 ) ) );

            m_Properties["releaseHooks"] = new BSCachedFunction(
                () => new BSFunctionReference( new BSFunction( "function releaseHook()", ReleaseHooksFunction, 0 ) ) );
        }


        private ABSObject HookFunction(ABSObject[] arg)
        {
            
                if (arg[0].ResolveReference() is BSFunction hook)
                {
                    AddHook(hook);

                    return BSObject.Null;
                }

                throw new BSInvalidTypeException(
                    SourcePosition.Unknown,
                    "Expected Function as argument.",
                    arg[0],
                    "BSFunction");
                
        }

        private ABSObject ReleaseHooksFunction(ABSObject[] arg)
        {
            ClearHooks();

            return BSObject.Null;
        }
        private ABSObject ReleaseHookFunction(ABSObject[] arg)
        {
            
                if (arg[0].ResolveReference() is BSFunction hook)
                {
                    RemoveHook(hook);

                    return BSObject.Null;
                }

                throw new BSInvalidTypeException(
                    SourcePosition.Unknown,
                    "Expected Function as argument.",
                    arg[0],
                    "BSFunction");
            }
        
        }


        #endregion
    }


