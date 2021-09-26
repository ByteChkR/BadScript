using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block.ForEach;
using BadScript.Common.Runtime;
using BadScript.Common.Types.References;
using BadScript.Common.Types.References.Implementations;

namespace BadScript.Common.Types.Implementations
{

    public class BSClassInstance : ABSObject, IEnumerable < IForEachIteration >
    {

        public BSScope InstanceScope => m_InstanceScope;
        private readonly BSScope m_InstanceScope;

        public readonly string Name;

        public override bool IsNull => false;
        

        #region Public
        
        public BSClassInstance(SourcePosition pos, string name, BSClassInstance baseInstance, BSScope instanceScope) : base(pos)
        {
            m_InstanceScope = instanceScope;
            if ( baseInstance != null )
                m_InstanceScope.AddLocalVar( "base" , baseInstance);
            m_InstanceScope.AddLocalVar("this" , this);
            Name = name;
        }

        

        public override bool Equals(ABSObject other)
        {
            return ReferenceEquals(this, other);
        }

        public IEnumerator<IForEachIteration> GetEnumerator()
        {
            return m_InstanceScope.GetEnumerator();
        }
        

        public override ABSReference GetProperty(string propertyName)
        {
            ABSObject k = new BSObject(propertyName);

            if (m_InstanceScope.HasLocal(propertyName))
            {
                return m_InstanceScope.Get(propertyName);
            }

            throw new BSRuntimeException( $"Property {propertyName} does not exist in Type '{Name}'" );
        }
        

        public override bool HasProperty(string propertyName)
        {
            return m_InstanceScope.HasLocal(propertyName);
        }
        
        public override ABSObject Invoke(ABSObject[] args)
        {
            throw new BSRuntimeException(Position, $"Can not invoke '{this}'");
        }

        public override string SafeToString(Dictionary<ABSObject, string> doneList)
        {
            if (doneList.ContainsKey(this))
            {
                return "<recursion>";
            }

            doneList[this] = "{}";

            StringWriter sw = new StringWriter();
            IndentedTextWriter tw = new IndentedTextWriter(sw);
            tw.WriteLine( $"class {Name}" );
            tw.WriteLine('{');

            foreach (IForEachIteration bsRuntimeIt in m_InstanceScope)
            {
                ABSObject[] objs = bsRuntimeIt.GetObjects();

                KeyValuePair < ABSObject, ABSObject > bsRuntimeObject =
                    new KeyValuePair < ABSObject, ABSObject >( objs[0], objs[1] );

                List<string> keyLines = bsRuntimeObject.Key.SafeToString(doneList).
                                                        Split(
                                                              new[] { '\n' },
                                                              StringSplitOptions.RemoveEmptyEntries
                                                             ).
                                                        Select(x => x.Trim()).
                                                        Where(x => !string.IsNullOrEmpty(x)).
                                                        ToList();

                List<string> valueLines = bsRuntimeObject.Value.SafeToString(doneList).
                                                          Split(
                                                                new[] { '\n' },
                                                                StringSplitOptions.RemoveEmptyEntries
                                                               ).
                                                          Select(x => x.Trim()).
                                                          Where(x => !string.IsNullOrEmpty(x)).
                                                          ToList();

                tw.Indent = 1;

                for (int i = 0; i < keyLines.Count; i++)
                {
                    string keyLine = keyLines[i];

                    if (i < keyLines.Count - 1)
                    {
                        tw.WriteLine(keyLine);
                    }
                    else
                    {
                        tw.Write(keyLine + " = ");
                    }
                }

                tw.Indent = 2;

                for (int i = 0; i < valueLines.Count; i++)
                {
                    string valueLine = valueLines[i];
                    tw.WriteLine(valueLine);
                }
            }

            tw.Indent = 0;
            tw.WriteLine('}');

            doneList[this] = sw.ToString();

            return doneList[this];
        }

        public override void SetProperty(string propertyName, ABSObject obj)
        {
            m_InstanceScope.Set(propertyName, obj);
        }

        public override bool TryConvertBool(out bool v)
        {
            v = false;

            return false;
        }

        public override bool TryConvertDecimal(out decimal d)
        {
            d = 0;

            return false;
        }

        public override bool TryConvertString(out string v)
        {
            v = null;

            return false;
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