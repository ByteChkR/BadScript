using System;
using System.Collections.Generic;

namespace BadScript.Tools.CodeGenerator.Runtime
{

    public abstract class WrapperStaticDataBase
    {
        public Dictionary < Type, BSStaticWrapperObject > StaticTypes;
        protected WrapperStaticDataBase()
        {
            StaticTypes = new Dictionary < Type, BSStaticWrapperObject>();
        }
    }

}