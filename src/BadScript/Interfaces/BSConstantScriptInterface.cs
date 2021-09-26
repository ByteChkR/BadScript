using System;

using BadScript.Common.Types;

namespace BadScript.Interfaces
{

    public class BSConstantScriptInterface : ABSScriptInterface
    {

        private readonly Action < ABSTable > m_AddApi;

        #region Public

        public BSConstantScriptInterface( string name, Action < ABSTable > addApi ) : base( name )
        {
            m_AddApi = addApi;
        }

        public override void AddApi( ABSTable root )
        {
            m_AddApi( root );
        }

        #endregion

    }

}
