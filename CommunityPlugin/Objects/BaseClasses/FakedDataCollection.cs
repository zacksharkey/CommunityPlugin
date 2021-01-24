using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using System;

namespace CommunityPlugin.Objects.BaseClasses
{
    public class FakedDataCollection : DataCollection
    {
        protected override object GetDataItemValue(string fieldID)
        {
            return EncompassHelper.CurrentLoan.Fields[fieldID].Value;
        }

        public override DataCollection.DataItem GetItem(string fieldID, IMapping mapping)
        {
            if (string.IsNullOrWhiteSpace(fieldID))
                throw new ArgumentNullException(nameof(fieldID));
            if (this.pitems.ContainsKey(fieldID))
                return this.pitems[fieldID];
            if (mapping != null)
            {
                switch (mapping.ValueType)
                {
                    case Enums.ValueType.Numeric:
                        return new DataCollection.DataItem()
                        {
                            FieldID = fieldID,
                            Value = (object)0,
                            ValueType = Enums.ValueType.Numeric
                        };
                    case Enums.ValueType.DateTime:
                        return new DataCollection.DataItem()
                        {
                            FieldID = fieldID,
                            Value = (object)DateTime.MinValue,
                            ValueType = Enums.ValueType.DateTime
                        };
                    default:
                        return new DataCollection.DataItem()
                        {
                            FieldID = fieldID,
                            Value = (object)string.Empty,
                            ValueType = Enums.ValueType.String
                        };
                }
            }
            else
                return new DataCollection.DataItem()
                {
                    FieldID = fieldID,
                    Value = (object)string.Empty,
                    ValueType = Enums.ValueType.String
                };
        }
    }
}
