using System;
using System.Collections.Generic;
using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Tools.CodeGenerator.Runtime
{

    public class BSWrapperObject<T> : ABSObject
    {

        protected T m_InternalObject;

        public T GetInternalObject() => m_InternalObject;
        protected Dictionary<string, ABSReference> m_Properties;
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
            if (other is BSWrapperObject<T> o)
                return ReferenceEquals(m_InternalObject, o.m_InternalObject);

            return false;
        }

        public override ABSReference GetProperty(string propertyName)
        {
            if (m_Properties.ContainsKey(propertyName))
            {
                return m_Properties[propertyName];
            }

            throw new BSRuntimeException("Invalid Property Name: " + propertyName);
        }

        public override bool HasProperty(string propertyName)
        {
            return m_Properties.ContainsKey(propertyName);
        }

        public override ABSObject Invoke(ABSObject[] args)
        {

            throw new BSRuntimeException("Can not Invoke Object");
        }

        public override string SafeToString(Dictionary<ABSObject, string> doneList)
        {
            return m_InternalObject?.ToString() ?? $"{typeof(T).Name}(NULL)";
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
            v = SafeToString();

            return true;
        }
    }
}
