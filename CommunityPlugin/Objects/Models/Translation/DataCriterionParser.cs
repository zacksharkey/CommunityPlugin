using CommunityPlugin.Objects.BaseClasses;
using CommunityPlugin.Objects.Interface;
using System;

namespace CommunityPlugin.Objects.Models.Translation
{
    public class DataCriterionParser : CriterionParser
    {
        public DataCollection Datas { get; private set; }

        public DataCriterionParser(DataCollection datas)
        {
            if (datas == null)
                throw new ArgumentNullException(nameof(datas));
            this.Datas = datas;
        }

        protected override DateTime GetCurrentTime()
        {
            try
            {
                return (DateTime)this.Datas.GetItem("$$now").Value;
            }
            catch (Exception ex)
            {
                return DateTime.Now;
            }
        }

        protected override IFunctionParser CreateFunctionParser()
        {
            IFunctionParser functionParser = (IFunctionParser)new DataFunctionParser(this.Datas);
            if (this.CustomFunctions != null)
                functionParser.CustomFunctions = this.CustomFunctions;
            return functionParser;
        }

        protected override string GetFieldValue(string fieldID, IMapping mapping)
        {
            if (string.IsNullOrEmpty(fieldID))
                throw new ArgumentNullException(nameof(fieldID));
            return this.Datas.GetString(fieldID, mapping);
        }
    }
}
 