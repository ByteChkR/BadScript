using BadScript.Common.Types;

namespace BadScript.Common.Expressions.Implementations.Block.ForEach
{

    public readonly struct ForEachIteration : IForEachIteration
    {
        private readonly ABSObject[] m_Objs;

        public ForEachIteration( ABSObject[] o )
        {
            m_Objs = o;
        }

        public ABSObject[] GetObjects()
        {
            return m_Objs;
        }
    }

}
