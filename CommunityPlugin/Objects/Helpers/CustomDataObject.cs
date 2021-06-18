using CommunityPlugin.Objects.Enums;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects;
using Newtonsoft.Json;
using System.Text;

namespace CommunityPlugin.Objects.Helpers
{
    public static class CustomDataObject
    {
        public static T Get<T>(CDOType Type = CDOType.Global, string user = "") where T : class, new()
        {
            T dataObject = new T();
            DataObject cdo = null;
            string key = Key<T>(dataObject);

            switch (Type)
            {
                case CDOType.Global:
                    cdo = EncompassApplication.Session.DataExchange.GetCustomDataObject(key);
                    break;
                case CDOType.Loan:
                    cdo = EncompassApplication.CurrentLoan.GetCustomDataObject(key);
                    break;
                case CDOType.User:
                    if (string.IsNullOrEmpty(user))
                        cdo = EncompassApplication.CurrentUser.GetCustomDataObject(key);
                    else
                        cdo = EncompassApplication.Session.Users.GetUser(user).GetCustomDataObject(key);
                    break;
            }
            if (cdo != null)
                dataObject = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(cdo.Data));
            else
                Save<T>(dataObject, Type);

            return dataObject;
        }


        public static void Save<T>(T Object, CDOType Type = CDOType.Global, string user = "")
        {
            byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Object));
            DataObject customDataObject = new DataObject(data);
            string key = Key<T>(Object);

            switch (Type)
            {
                case CDOType.Global:
                    EncompassApplication.Session.DataExchange.SaveCustomDataObject(key, customDataObject);
                    break;
                case CDOType.Loan:
                    EncompassApplication.CurrentLoan.SaveCustomDataObject(key, customDataObject);
                    break;
                case CDOType.User:
                    if (string.IsNullOrEmpty(user))
                        EncompassApplication.CurrentUser.SaveCustomDataObject(key, customDataObject);
                    else
                        EncompassApplication.Session.Users.GetUser(user).SaveCustomDataObject(key, customDataObject);
                    break;
            }
        }

        private static string Key<T>(T Object)
        {
            return $"{Object.GetType().Name}.json";
        }
    }
}