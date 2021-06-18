using CommunityPlugin.Non_Native_Modifications.SideMenu.Controls;
using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Enums;
using CommunityPlugin.Objects.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CommunityPlugin.Objects.CustomDataObjects;

namespace CommunityPlugin.Non_Native_Modifications.SideMenu.UserControls
{
    public class LinksAndResources : LoanMenuControl
    {
        public Dictionary<string, Dictionary<string, string>> Links { get; set; }
        private FlowLayoutPanel layout;
        public override bool CanRun()
        {
            return PluginAccess.CheckAccess(nameof(LinksAndResources));
        }

        public override bool CanShow()
        {
            return CanRun();
        }

        public LinksAndResources()
        {
            RefreshControl();
            CommunitySettings cdo = CustomDataObject.Get<CommunitySettings>();
            var rights = cdo.Rights;
            CustomDataObject.Save<CommunitySettings>(cdo);

            //Links = cdo.Links == null || cdo.Links.Count.Equals(0) ? new Dictionary<string, Dictionary<string, string>>() : cdo.Links.ContainsKey(EncompassHelper.LastPersona) ? cdo.Links[EncompassHelper.LastPersona] : cdo.Links["Default"];
        }
        private void RefreshControl()
        {
            this.Name = "Links And Resources";
            List<CustomLinkLabel> links = GetLinkLabels();

            this.Size = new System.Drawing.Size(320, ((links.Count + 3) * 28));

            layout = new FlowLayoutPanel();
            layout.Size = this.Size;
            layout.FlowDirection = FlowDirection.TopDown;

            AddToList(links, LinkType.EFolder);
            AddToList(links, LinkType.Pricing);
            AddToList(links, LinkType.Popup);
            AddToList(links, LinkType.Internet);
            AddToList(links, LinkType.Internet, true);
            AddToList(links, LinkType.Form);
            AddToList(links, LinkType.Service);

            this.Controls.Add(layout);
        }

        private List<CustomLinkLabel> GetLinkLabels()
        {
            List<CustomLinkLabel> result = new List<CustomLinkLabel>();
            if (Links == null)
                return result;

            foreach(KeyValuePair<string, Dictionary<string, string>> kvp in Links)
            {
                foreach(KeyValuePair<string,string> key in kvp.Value)
                {
                    LinkType type = (LinkType)Enum.Parse(typeof(LinkType), kvp.Key);
                    string text = key.Value;
                    string formName = !type.Equals(LinkType.Internet) ? key.Value : string.Empty;
                    int popupWidth = type.Equals(LinkType.Popup) ? 900 : 0;
                    int popupHeight = type.Equals(LinkType.Popup) ? 900 : 0;
                    string internetLink = type.Equals(LinkType.Internet) ? key.Value : string.Empty;

                    result.Add(new CustomLinkLabel(type, text, formName, popupWidth, popupHeight, internetLink));
                }
            }

            return result;
        }

        private void AddToList(List<CustomLinkLabel> Links, LinkType Type, bool IsMI = false)
        {
            List<CustomLinkLabel> temp;
            if (Type == LinkType.Internet && IsMI)
                temp = Links.Where(x => x.Type.Equals(Type) && x.IsMi).ToList();
            else if (Type == LinkType.Internet && !IsMI)
                temp = Links.Where(x => x.Type.Equals(Type) && !x.IsMi).ToList();
            else
                temp = Links.Where(x => x.Type.Equals(Type)).ToList();

            if (temp.Count > 0)
            {
                layout.Controls.Add(TextPanel($"{ControlHeader(Type, IsMI)}:", 17));
                AddLinks(temp);
            }
        }

        private string ControlHeader(LinkType Type, bool IsMI = false)
        {
            switch (Type)
            {
                case LinkType.EFolder:
                    return "eFolder Links";
                case LinkType.Popup:
                    return "Pop Up";
                case LinkType.Form:
                    return "Forms";
                case LinkType.Service:
                    return "Services";
                case LinkType.Pricing:
                    return "Pricing";
                case LinkType.Internet:
                    return IsMI ? "Mortgage Insurance" : "Internet Links";
                default:
                    return string.Empty;
            }
        }


        /// <summary>
        /// Add links to layout
        /// </summary>
        /// <param name="Links"></param>
        private void AddLinks(List<CustomLinkLabel> Links)
        {
            if (layout == null)
                return;

            foreach (CustomLinkLabel link in Links)
                layout.Controls.Add(new CustomLinkLabel(link.Type, link.Text, link.FormName, link.PopupWidth, link.PopupHeight, link.InternetLink));
        }

        private Panel TextPanel(string text, int div, bool isHeader = false)
        {
            Panel panel = new Panel();
            panel.Height = div;
            Label maxLabel = new Label();
            maxLabel.Text = text;
            maxLabel.Dock = DockStyle.Fill;
            maxLabel.Font = new System.Drawing.Font(maxLabel.Font.FontFamily, 11, System.Drawing.FontStyle.Bold);
            panel.Controls.Add(maxLabel);

            return panel;
        }
    }
}