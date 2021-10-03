using System.Collections;
using System.Collections.Generic;

using BadScript.Parser.Expressions.Implementations.Block.ForEach;
using BadScript.Types.Implementations;

namespace BadScript.Examples.Integration
{

    public class NameListWrapper :
        BSObject, //Override BSObject to have all members already overridden with their default implementations
        IEnumerable < IForEachIteration > //Implement this interface to be able to "foreach" on the object inside the script
    {

        private readonly NameList m_List;

        #region Public

        public NameListWrapper( NameList list ) : base( list )
        {
            m_List = list;
        }

        public IEnumerator < IForEachIteration > GetEnumerator()
        {
            //Loop through the names and yield them
            foreach ( string listName in m_List.Names )
            {
                yield return new ForEachIteration( new BSObject( listName ) );
            }
        }

        #endregion

        #region Private

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }

}
