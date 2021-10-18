using BadScript.Types;

namespace BadScript.Profiling
{

    public class BSFunctionInvocationData
    {

        public readonly long StartTicks;

        public readonly BSFunction Function;
        public readonly ABSObject[] Args;

        public long EndTicks { get; private set; }

        #region Public

        public BSFunctionInvocationData( BSFunction f, ABSObject[] args, long startTicks )
        {
            StartTicks = startTicks;
            Args = args;
            Function = f;
        }

        public void SetEnd( long endTicks )
        {
            EndTicks = endTicks;
        }

        #endregion

    }

}
