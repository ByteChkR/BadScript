﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Types;
using BadScript.Common.Types.References;

namespace BadScript.Tools.CodeGenerator.Runtime
{

    public class BSStaticWrapperObject : ABSObject
    {
        protected Dictionary < string, ABSReference > m_StaticProperties;

        public string[] Properties => m_StaticProperties.Keys.ToArray();
        private Type m_WrappedType;
        public BSStaticWrapperObject(Type t) : base( SourcePosition.Unknown )
        {
            m_WrappedType = t;
            m_StaticProperties = new Dictionary < string, ABSReference >();
        }

        public override bool IsNull => false;

        public override bool Equals(ABSObject other)
        {
            return ReferenceEquals( this, other );
        }

        public override ABSReference GetProperty(string propertyName)
        {
            if (m_StaticProperties.ContainsKey(propertyName))
            {
                return m_StaticProperties[propertyName];
            }

            throw new BSRuntimeException("Invalid Property Name: " + propertyName);
        }

        public override bool HasProperty(string propertyName)
        {
            return m_StaticProperties.ContainsKey(propertyName);
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

            StringWriter sw = new StringWriter();
            IndentedTextWriter tw = new IndentedTextWriter(sw);
            tw.WriteLine('{');

            foreach (KeyValuePair<string, ABSReference> bsRuntimeObject in m_StaticProperties)
            {
                List<string> keyLines = bsRuntimeObject.Key.
                                                        Split(
                                                            new[] { '\n' },
                                                            StringSplitOptions.RemoveEmptyEntries
                                                        ).
                                                        Select(x => x.Trim()).
                                                        Where(x => !string.IsNullOrEmpty(x)).
                                                        ToList();

                List<string> valueLines = bsRuntimeObject.Value.ResolveReference().SafeToString(doneList).
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

            return sw.ToString();
        }

        public override void SetProperty(string propertyName, ABSObject obj)
        {
            if (m_StaticProperties.ContainsKey(propertyName))
            {
                m_StaticProperties[propertyName].Assign(obj);

                return;
            }
            throw new BSRuntimeException("Object does not support writing properties");
        }

        public override bool TryConvertBool(out bool v)
        {
            v = true;

            return true;
        }

        public override bool TryConvertDecimal(out decimal d)
        {
            d = 0;

            return false;
        }

        public override bool TryConvertString(out string v)
        {

            v = m_WrappedType.Name;

            return true;
        }
        
    }

}
