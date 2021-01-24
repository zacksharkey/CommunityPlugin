using CommunityPlugin.Objects.BaseClasses;
using System;

namespace CommunityPlugin.Objects.Models.Translation
{
    public class DataFunctionParser : FunctionParser
    {
        public DataCollection Datas { get; private set; }

        public DataFunctionParser(DataCollection datas)
        {
            if (datas == null)
                throw new ArgumentNullException(nameof(datas));
            this.Datas = datas;
        }
    }
}
