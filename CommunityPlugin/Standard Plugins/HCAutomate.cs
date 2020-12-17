using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using CommunityPlugin.Objects.Models;
using EllieMae.EMLite.Common;
using EllieMae.EMLite.DataEngine;
using EllieMae.Encompass.BusinessObjects.Loans;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;

namespace CommunityPlugin.Standard_Plugins
{
    public class HCAutomate : Plugin, IFieldChange, ILoanOpened
    {
        private void ClearAndUpdateData()
        {
            GeoCoordinate coordinates = Coordinates();


            List<HomeCounselor> List = GetAgencies(coordinates);
            if(List != null)
            {
                int index = 1;
                foreach(HomeCounselor c in List)
                {
                    EncompassHelper.SetBlank("HC01", index.ToString());
                    EncompassHelper.SetBlank("HC02", index.ToString());
                    EncompassHelper.SetBlank("HC03", index.ToString());
                    EncompassHelper.SetBlank("HC04", index.ToString());
                    EncompassHelper.SetBlank("HC05", index.ToString());
                    EncompassHelper.SetBlank("HC06", index.ToString());
                    EncompassHelper.SetBlank("HC07", index.ToString());
                    EncompassHelper.SetBlank("HC09", index.ToString());
                    EncompassHelper.SetBlank("HC10", index.ToString());
                    EncompassHelper.SetBlank("HC11", index.ToString());
                    EncompassHelper.SetBlank("HC12", index.ToString());
                    EncompassHelper.SetBlank("HC13", index.ToString());
                    EncompassHelper.SetBlank("HC17", index.ToString());

                    index++;
                }

                index = 1;
                foreach (HomeCounselor c in List)
                {
                    string i = index.ToString();
                    EncompassHelper.Set("HC01", !string.IsNullOrEmpty(c.Name) ? "Y" : string.Empty, i);
                    EncompassHelper.Set("HC02", c.Name, i);
                    EncompassHelper.Set("HC03", c.Address1, i);
                    EncompassHelper.Set("HC04", c.City, i);
                    EncompassHelper.Set("HC05", c.State, i);
                    EncompassHelper.Set("HC06", c.ZipCode, i);
                    EncompassHelper.Set("HC07", c.Phone1, i);
                    EncompassHelper.Set("HC09", c.Fax, i);
                    EncompassHelper.Set("HC10", c.Email, i);
                    EncompassHelper.Set("HC11", c.WebURL, i);
                    EncompassHelper.Set("HC12", Translate(c.Languages), i);
                    EncompassHelper.Set("HC13", Translate(c.Services, true), i);
                    EncompassHelper.Set("HC17", GetDistance(coordinates.Latitude, coordinates.Longitude, c.Latitude, c.Longitude), i);

                    index++;
                }
            }
        }

        private string Translate(string Array, bool services = false)
        {
            List<KeyValuePair<string, string>> Languages = EncompassHelper.SessionObjects.ConfigurationManager.GetHomeCounselingLanguageSupported();
            List<KeyValuePair<string, string>> Services = EncompassHelper.SessionObjects.ConfigurationManager.GetHomeCounselingServiceSupported();
            List<KeyValuePair<string, string>>[] LanguagesSupported = EncompassHelper.SessionObjects.ConfigurationManager.GetHomeCounselingServiceLanguageSupported();

            char sign = services ? '|' : ',';
            StringBuilder sb = new StringBuilder();
            foreach (string item in Array.Split(','))
            {
                string converted = services ? Services.FirstOrDefault(x => x.Key.Equals(item)).Value : Languages.FirstOrDefault(x => x.Key.Equals(item)).Value;
                if (!string.IsNullOrEmpty(converted))
                {
                    sb.Append($"{converted}{sign}");
                }
            }

            return sb.ToString().Substring(0, sb.Length - 1);
        }

        private string GetDistance(double lat, double lng, string lat2, string lng2)
        {
            try
            {
                double.TryParse(lat2, out double l2);
                double.TryParse(lng2, out double lo2);
                GeoCoordinate coord = new GeoCoordinate(lat, lng);
                return Utils.ArithmeticRounding(coord.GetDistanceTo(new GeoCoordinate(l2, lo2)) / 1609.34, 2).ToString();
            }
            catch(Exception ex)
            {
                Logger.HandleError(ex, nameof(HCAutomate));
                return "0";
            }
        }

        private GeoCoordinate Coordinates()
        {
            string city = EncompassHelper.Val("FR0106");
            string state = EncompassHelper.Val("FR0107");
            string zipcode = EncompassHelper.Val("FR0108");

            return ZipCodeUtils.GetZipGeoCoordinate(zipcode, state, city, string.Empty);
        }

        private List<HomeCounselor> GetAgencies(GeoCoordinate Coordinates)
        {
            string uri = $"https://data.hud.gov/Housing_Counselor/searchByLocation?Lat={Coordinates.Latitude}&Long={Coordinates.Longitude}&Distance=500&RowLimit=&Services=&Languages=";
            List<HomeCounselor> result = JsonConvert.DeserializeObject<List<HomeCounselor>>(WebRequest.Request(uri));
            if (result == null || result.Count() < 10)
                return null;

            return result.Take(10).ToList();
        }

        public override void LoanOpened(object sender, EventArgs e)
        {

        }

        public override void FieldChanged(object sender, FieldChangeEventArgs e)
        {
            bool flag = string.IsNullOrEmpty(EncompassHelper.Val("3152"));
            bool flag2 = e.FieldID.Equals("FR0108");
            bool flag3 = !string.IsNullOrEmpty(e.NewValue);
            bool flag4 = e.FieldID.Equals("3142");

            if (flag && flag3 && (flag2 || flag4))
            {
                ClearAndUpdateData();
            }
        }
    }
}
