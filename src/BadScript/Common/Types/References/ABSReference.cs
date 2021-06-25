using System.Collections.Generic;

namespace BadScript.Common.Types.References
{

    public abstract class ABSReference : ABSObject
    {
        #region Public

        public override bool IsNull => Get().IsNull;

        public abstract void Assign( ABSObject obj );

        public abstract ABSObject Get();


        public override bool Equals(ABSObject other)
        {
            return Get().Equals(other);
        }


        public override ABSReference GetProperty(string propertyName)
        {
            return Get().GetProperty(propertyName);
        }

        public override bool HasProperty(string propertyName)
        {
            return Get().HasProperty(propertyName);
        }

        public override ABSObject Invoke(ABSObject[] args)
        {
            return Get().Invoke(args);
        }

        public override string SafeToString(Dictionary<ABSObject, string> doneList)
        {
            return Get().SafeToString(doneList);
        }

        public override void SetProperty(string propertyName, ABSObject obj)
        {
            Get().SetProperty(propertyName, obj);
        }

        public override bool TryConvertBool(out bool v)
        {
            return Get().TryConvertBool(out v);
        }

        public override bool TryConvertDecimal(out decimal d)
        {
            return Get().TryConvertDecimal(out d);
        }

        public override bool TryConvertString(out string v)
        {
            return Get().TryConvertString(out v);
        }

        #endregion

    }

}
