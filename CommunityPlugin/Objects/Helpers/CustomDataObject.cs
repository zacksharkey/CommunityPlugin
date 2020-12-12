using CommunityPlugin.Objects.Enums;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects;
using Newtonsoft.Json;
using System.Text;

namespace CommunityPlugin.Objects.Helpers
{
    public static class CustomDataObject
    {
        public static T Get<T>(string Key, CDOType Type = CDOType.Global) where T : class, new()
        {
            T dataObject = new T();
            DataObject cdo = null;

            switch (Type)
            {
                case CDOType.Global:
                    cdo = EncompassApplication.Session.DataExchange.GetCustomDataObject(Key);
                    break;
                case CDOType.Loan:
                    cdo = EncompassApplication.CurrentLoan.GetCustomDataObject(Key);
                    break;
                case CDOType.User:
                    cdo = EncompassApplication.CurrentUser.GetCustomDataObject(Key);
                    break;
            }
            if (cdo != null)
                dataObject =  JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(cdo.Data));
            else
                Save<T>(Key, dataObject, Type);

            return dataObject;
        }


        public static void Save<T>(string Key, T Object, CDOType Type = CDOType.Global)
        {
            byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Object));
            DataObject customDataObject = new DataObject(data);

            switch (Type)
            {
                case CDOType.Global:
                    EncompassApplication.Session.DataExchange.SaveCustomDataObject(Key, customDataObject);
                    break;
                case CDOType.Loan:
                    EncompassApplication.CurrentLoan.SaveCustomDataObject(Key, customDataObject);
                    break;
                case CDOType.User:
                    EncompassApplication.CurrentUser.SaveCustomDataObject(Key, customDataObject);
                    break;
            }
        }
    }
}
