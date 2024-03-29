﻿using BadScript.Types;

namespace BadScript.Parser.Expressions.Implementations.Block.ForEach
{

    public readonly struct ForEachIteration : IForEachIteration
    {

        private readonly ABSObject[] m_Objs;

        public ForEachIteration( params ABSObject[] o )
        {
            m_Objs = o;
        }

        public ABSObject[] GetObjects()
        {
            return m_Objs;
        }

    }

}
