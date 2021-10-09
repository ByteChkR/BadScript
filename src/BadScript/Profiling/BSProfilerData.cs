using System;
using System.Collections.Generic;
using System.Diagnostics;

using BadScript.Types;

using Newtonsoft.Json;

namespace BadScript.Profiling
{

    public static class BSProfilerData
    {

        private class BSSerializedProfilerData
        {

            public string Name;
            public long Calls;
            public decimal AverageMillis;
            public decimal MinMillis;
            public decimal MaxMillis;

        }

        private static readonly Stopwatch m_Stopwatch = new Stopwatch();

        private static readonly Dictionary < string, BSProfiledFunctionData > m_FunctionData =
            new Dictionary < string, BSProfiledFunctionData >();

        private static bool s_EnableProfiler;

        public static bool EnableProfiler
        {
            get => s_EnableProfiler;
            set
            {
                if ( value )
                {
                    Start();
                }
                else
                {
                    End();
                }

                s_EnableProfiler = value;
            }
        }

        #region Unity Event Functions

        public static void Start()
        {
            m_Stopwatch.Restart();
        }

        #endregion

        #region Public

        public static void End()
        {
            m_Stopwatch.Stop();
        }

        public static BSFunctionInvocationData ProfilerFunctionEntry( BSFunction f, ABSObject[] args )
        {
            BSProfiledFunctionData fData;
            string fStr = f.SafeToString();

            if ( m_FunctionData.ContainsKey( fStr ) )
            {
                fData = m_FunctionData[fStr];
            }
            else
            {
                fData = m_FunctionData[fStr] = new BSProfiledFunctionData();
            }

            BSFunctionInvocationData invData = new BSFunctionInvocationData( f, args, m_Stopwatch.ElapsedTicks );
            fData.Invocations.Add( invData );

            return invData;
        }

        public static void ProfilerFunctionLeave( BSFunctionInvocationData f )
        {
            f.SetEnd( m_Stopwatch.ElapsedTicks );
        }

        public static string SerializeProfilerData()
        {
            List < BSSerializedProfilerData > data = new List < BSSerializedProfilerData >();

            foreach ( KeyValuePair < string, BSProfiledFunctionData > d in m_FunctionData )
            {
                decimal min = decimal.MaxValue;
                decimal max = 0;
                decimal avg = 0;

                foreach ( BSFunctionInvocationData id in d.Value.Invocations )
                {
                    decimal millis = ( id.EndTicks - id.StartTicks ) / ( decimal )TimeSpan.TicksPerMillisecond;

                    if ( min > millis )
                    {
                        min = millis;
                    }

                    if ( max < millis )
                    {
                        max = millis;
                    }

                    avg += millis;
                }

                avg /= d.Value.Invocations.Count;

                BSSerializedProfilerData da = new BSSerializedProfilerData
                                              {
                                                  Name = d.Key,
                                                  AverageMillis = avg,
                                                  MaxMillis = max,
                                                  MinMillis = min,
                                                  Calls = d.Value.Invocations.Count
                                              };

                data.Add( da );
            }

            return JsonConvert.SerializeObject( data, Formatting.Indented );
        }

        #endregion

    }

}
