using System;
using BadScript.Exceptions;
using BadScript.Parser.Expressions;
using BadScript.Types;
using BadScript.Types.References;

namespace BadScript.Reflection;

public class BSReflectionReference : ABSReference
{
    private readonly ABSObject m_ErrorFallback;

    private readonly Func<ABSObject> m_Getter;
    private readonly Action<ABSObject> m_Setter;

    #region Protected

    protected override int GetHashCodeImpl()
    {
        return m_Getter.GetHashCode() ^ (m_Setter?.GetHashCode() ?? 0);
    }

    #endregion

    #region Public

    public BSReflectionReference(Func<ABSObject> get, Action<ABSObject> set, ABSObject mErrorFallback = null) : base(
        SourcePosition.Unknown
    )
    {
        m_Getter = get;
        m_Setter = set;
        m_ErrorFallback = mErrorFallback;
    }

    public override void Assign(ABSObject obj)
    {
        if (m_Setter == null) throw new BSRuntimeException("Reflection Reference is Readonly");

        m_Setter(obj);
    }

    public override ABSObject Get()
    {
        if (m_ErrorFallback != null)
            try
            {
                return m_Getter();
            }
            catch (Exception e)
            {
                return m_ErrorFallback;
            }

        return m_Getter();
    }

    #endregion
}