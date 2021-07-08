using System;
using BadScript.Common.Types;

namespace BadScript
{

    public class BSConstantScriptInterface : ABSScriptInterface
    {
        private readonly Action < ABSTable > m_AddApi;
        public BSConstantScriptInterface( string name, Action <ABSTable> addApi ) : base( name )
        {
            m_AddApi = addApi;
        }

        public override void AddApi( ABSTable root ) => m_AddApi( root );
    }

}