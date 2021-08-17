using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block.ForEach;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Tools.CodeGenerator.Runtime
{
    
    public class BSWrapperObject<T> : ABSObject, IBSWrappedObject, IEnumerable<IForEachIteration>
    {

        protected T m_InternalObject;
        

        public T GetInternalObject() => m_InternalObject;
        protected Dictionary<string, ABSReference> m_Properties = new Dictionary < string, ABSReference >();
        
        public BSWrapperObject(T obj) : base(SourcePosition.Unknown)
        {
            m_InternalObject = obj;

            m_Properties.Add(
                "ToString",
                new BSFunctionReference(
                    new BSFunction("function ToString()", x => new BSObject(SafeToString()), 0)));


        }

        public override bool IsNull => m_InternalObject == null;

        public override bool Equals(ABSObject other)
        {
            if ( other == null )
                return false;
            if (IsNull) return other.IsNull;
            if (other is BSWrapperObject<T> o)
            {
                return m_InternalObject.Equals( o.m_InternalObject );
                //return m_InternalObject == o.m_InternalObject;
                //return ReferenceEquals( m_InternalObject, o.m_InternalObject );
            }
            return false;
        }

        public override ABSReference GetProperty(string propertyName)
        {
            if (!IsNull && m_Properties.ContainsKey(propertyName))
            {
                return m_Properties[propertyName];
            }

            throw new BSRuntimeException("Invalid Property Name: " + propertyName);
        }

        public override bool HasProperty(string propertyName)
        {
            if ( IsNull )
                return false;
            return m_Properties.ContainsKey(propertyName);
        }

        public override ABSObject Invoke(ABSObject[] args)
        {

            throw new BSRuntimeException("Can not Invoke Object");
        }
        public override string SafeToString(Dictionary<ABSObject, string> doneList)
        {
            if (doneList.ContainsKey(this))
            {
                return "<recursion>";
            }

            doneList[this] = "{}";

            if ( IsNull )
                return "NULL";

              StringWriter sw = new StringWriter();
            IndentedTextWriter tw = new IndentedTextWriter(sw);
            tw.WriteLine('{');

            foreach (KeyValuePair<string, ABSReference> bsRuntimeObject in m_Properties)
            {
                List<string> keyLines = bsRuntimeObject.Key.
                                                           Split(
                                                               new[] { '\n' },
                                                               StringSplitOptions.RemoveEmptyEntries
                                                           ).
                                                           Select(x => x.Trim()).
                                                           Where(x => !string.IsNullOrEmpty(x)).
                                                           ToList();

                List < string > valueLines = new List < string >();

                if(WrapperHelper.AllowRecurseToString)
                {
                    ABSObject resolvedValue = bsRuntimeObject.Value.ResolveReference();
                    
                    if ( resolvedValue is IBSWrappedObject wo )
                    {
                        valueLines = wo.GetInternalObject().ToString().Split('\n').ToList();
                    }
                    else
                    {
                        valueLines = resolvedValue.SafeToString(doneList).
                                                   Split(
                                                       new[] { '\n' },
                                                       StringSplitOptions.RemoveEmptyEntries
                                                   ).
                                                   Select(x => x.Trim()).
                                                   Where(x => !string.IsNullOrEmpty(x)).
                                                   ToList();
                    }
                }

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
                        if(valueLines.Count != 0)
                        {
                            tw.Write(keyLine + " = ");
                        }
                        else
                        {
                            tw.WriteLine(keyLine + " = ");
                        }
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

            return sw.ToString();
        }

        public override void SetProperty(string propertyName, ABSObject obj)
        {
            if (m_Properties.ContainsKey(propertyName))
            {
                m_Properties[propertyName].Assign(obj);

                return;
            }
            throw new BSRuntimeException("Object does not support writing properties");
        }

        public override bool TryConvertBool(out bool v)
        {
            v = m_InternalObject != null;

            return true;
        }

        public override bool TryConvertDecimal(out decimal d)
        {
            d = 0;

            return false;
        }

        public override bool TryConvertString(out string v)
        {

            v= m_InternalObject?.ToString() ?? $"{typeof(T).Name}(NULL)";

            return true;
        }

        object IBSWrappedObject.GetInternalObject()
        {
            return GetInternalObject();
        }

        public IEnumerator < IForEachIteration > GetEnumerator()
        {
            foreach (KeyValuePair<string, ABSReference> keyValuePair in m_Properties)
            {
                yield return new ForEachIteration(new ABSObject[] { new BSObject(keyValuePair.Key)});
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
