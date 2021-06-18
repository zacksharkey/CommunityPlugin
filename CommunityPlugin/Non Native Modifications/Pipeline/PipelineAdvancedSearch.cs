using CommunityPlugin.Objects;
using CommunityPlugin.Objects.CustomDataObjects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using CommunityPlugin.Objects.Models;
using EllieMae.EMLite.ClientServer;
using EllieMae.EMLite.ClientServer.Reporting;
using EllieMae.EMLite.Common;
using EllieMae.EMLite.MainUI;
using EllieMae.EMLite.UI.Controls;
using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications.Pipeline
{
    public class PipelineAdvancedSearch : Plugin, IPipelineTabChanged
    {
        private ComboBox Filter;
        private CheckedComboBox Folder;
        private ComboBox View;
        private PipelineFilterCDO cdo;

        public override void PipelineTabChanged(object sender, EventArgs e)
        {
            cdo = CustomDataObject.Get<PipelineFilterCDO>();
            if (cdo == null)
                return;

            TabControl tab = sender as TabControl;
            TabPage tabPage = tab.TabPages[tab.SelectedIndex];
            Folder = tabPage.Controls.Find("cboFolder", true)[0] as CheckedComboBox;
            View = tabPage.Controls.Find("cboView", true)[0] as ComboBox;
            View.SelectedIndexChanged += View_SelectedIndexChanged;
            Control c = tabPage.Controls.Find("gradientPanel1", true).FirstOrDefault();
            if (c != null && c.Controls.Find("save", true).Count().Equals(0))
            {
                Filter = new ComboBox();
                Label label = new Label();
                Button save = new Button();
                Button delete = new Button();
                c.Controls.Add(label);
                c.Controls.Add(Filter);
                c.Controls.Add(save);
                c.Controls.Add(delete);
                Point p = c.Controls.Find("btnManageViews", true).FirstOrDefault().Location;
                label.Location = new Point(p.X + 20, p.Y);
                label.Text = "Filters";
                label.Size = new Size(label.Size.Width / 2, label.Size.Height);
                Filter.Location = new Point(label.Location.X + label.Width + 5, label.Location.Y);
                Filter.SelectedValueChanged += Filter_SelectedValueChanged;
                save.Location = new Point(Filter.Location.X + Filter.Width + 5, Filter.Location.Y);
                save.Click += Save_Click;
                save.Text = "Save";
                delete.Location = new Point(save.Location.X + save.Width + 5, save.Location.Y);
                delete.Click += Delete_Click;
                delete.Text = "Delete";

                LoadFilters(Filter);
            }
        }

        private void View_SelectedIndexChanged(object sender, EventArgs e)
        {
            Filter.Text = string.Empty;
            LoadFilters(Filter);
        }

        private void Filter_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            PipelineScreen mainScreen = FormWrapper.EncompassForm.Controls.Find("pipelineScreen", true)[0] as PipelineScreen;
            PipelineFilter filter = cdo.Filters.Where(x => x.Name.Equals(combo.Text)).FirstOrDefault();
            mainScreen.SetCurrentFilter(filter.Filter, 1);

            ClientCommonUtils.UncheckLoanFolders(this.Folder, Folder.CheckedItems.Cast<ComboBoxItem>().ToList());
            mainScreen.SetCurrentFolder(filter.Folder);
            mainScreen.RefreshPipeline();
        }
        private void LoadViewFolders(string folderName, PipelineScreen screen)
        {
            if (string.IsNullOrEmpty(folderName))
                return;
            string[] folderList = folderName.Split('|');
            LoanFolderInfo[] info = folderList.Select(x => EncompassHelper.SessionObjects.LoanManager.GetLoanFolder(x)).ToArray<LoanFolderInfo>();
            MethodInfo select = screen.GetType().GetMethod("selectFolders", BindingFlags.NonPublic | BindingFlags.Instance);
            select.Invoke(screen, new object[] { info });
            MethodInfo refresh = screen.GetType().GetMethod("refreshFilterDescription", BindingFlags.NonPublic | BindingFlags.Instance);
            refresh.Invoke(screen, new object[] { });
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            PipelineScreen mainScreen = FormWrapper.EncompassForm.Controls.Find("mainScreen", true)[0] as PipelineScreen;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            PipelineScreen mainScreen = FormWrapper.EncompassForm.Controls.Find("pipelineScreen", true)[0] as PipelineScreen;
            FieldFilterList filter = mainScreen.GetCurrentFilter();

            cdo.Filters.Add(new PipelineFilter()
            {
                Name = Filter.Text,
                Filter = filter,
                Public = false,
                Owner = EncompassHelper.User.ID,
                Folder = GetSelectedFolderList(),
                PipelineView = View.Text
            });

            CustomDataObject.Save<PipelineFilterCDO>(cdo);
            LoadFilters(Filter);
            Filter.Text = string.Empty;
        }

        private void LoadFilters(ComboBox Filter)
        {
            Filter.Items.Clear();
            Filter.Items.AddRange(cdo.Filters.Where(x => x.Public && x.PipelineView.Equals(View.Text) || (!x.Public && x.Owner.Equals(EncompassHelper.User.ID) && x.PipelineView.Equals(View.Text))).Select(x => x.Name).ToArray());
        }

        private string GetSelectedFolderList()
        {
            if (string.IsNullOrEmpty(Folder.ComboBoxText) && Folder.ItemList.CheckedItems.Count == 0)
                return "";
            StringBuilder stringBuilder = new StringBuilder();
            foreach (object checkedItem in Folder.ItemList.CheckedItems)
            {
                stringBuilder.Append(((ComboBoxItem)checkedItem).Name);
                stringBuilder.Append("|");
            }
            if (stringBuilder.Length > 0)
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            return stringBuilder.ToString();
        }
    }
}
