using BadScript.Types;

namespace BadScript.Profiling
{

    public class BSFunctionInvocationData
    {

        public readonly long StartTicks;
        public long EndTicks { get; private set; }

        public readonly BSFunction Function;
        public readonly ABSObject[] Args;

        public BSFunctionInvocationData( BSFunction f, ABSObject[] args, long startTicks )
        {
            StartTicks = startTicks;
            Args = args;
            Function = f;
        }
        public void SetEnd( long endTicks ) => EndTicks = endTicks;

    }

}