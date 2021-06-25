using System;
using System.Collections.Generic;
using System.Linq;
using BadScript.Common.Exceptions;
using BadScript.Common.OperatorImplementations;
using BadScript.Common.OperatorImplementations.Implementations;
using BadScript.Common.Types;
using BadScript.Common.Types.References;

namespace BadScript.Common.Runtime
{

    public static class BSOperatorImplementationResolver
    {
        private static List < ABSOperatorImplementation > m_Implementations;

        #region Public

        public static void AddImplementation( ABSOperatorImplementation o )
        {
            m_Implementations.Add( o );
        }

        public static ABSOperatorImplementation ResolveImplementation( string key, ABSObject[] args )
        {
            for ( int i = 0; i < args.Length; i++ )
            {
                args[i] = args[i].ResolveReference();
            }

            ABSOperatorImplementation imp = m_Implementations.
                LastOrDefault( x => x.OperatorKey == key && x.IsCorrectImplementation( args ) );

            if ( imp != null )
            {
                return imp;
            }

            throw new BSRuntimeException( $"Could not find operator({key}) implementation" );
        }

        #endregion

        #region Private

        static BSOperatorImplementationResolver()
        {
            m_Implementations = new List < ABSOperatorImplementation >();
            m_Implementations.Add( new BSAddOperatorImplementation() );
            m_Implementations.Add( new BSAndOperatorImplementation() );
            m_Implementations.Add( new BSDivideOperatorImplementation() );
            m_Implementations.Add( new BSEqualityOperatorImplementation() );
            m_Implementations.Add( new BSGreaterOrEqualOperatorImplementation() );
            m_Implementations.Add( new BSGreaterThanOperatorImplementation() );
            m_Implementations.Add( new BSInEqualityOperatorImplementation() );
            m_Implementations.Add( new BSLessOrEqualOperatorImplementation() );
            m_Implementations.Add( new BSLessThanOperatorImplementation() );
            m_Implementations.Add( new BSMinusOperatorImplementation() );
            m_Implementations.Add( new BSModuloOperatorImplementation() );
            m_Implementations.Add( new BSMultiplyOperatorImplementation() );
            m_Implementations.Add( new BSNotOperatorImplementation() );
            m_Implementations.Add( new BSOrOperatorImplementation() );
            m_Implementations.Add( new BSXOrDefaultOperatorImplementation() );
            m_Implementations.Add( new BSNullTestOperatorImplementation() );
        }

        #endregion
    }

}
