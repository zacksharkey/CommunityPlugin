using CommunityPlugin.Objects;
using CommunityPlugin.Objects.CustomDataObjects;
using CommunityPlugin.Objects.Helpers;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu
{
    public partial class PluginManagement_Form : Form
    {
        public PluginManagement_Form()
        {
            InitializeComponent();
            CommunitySettings cdo = CustomDataObject.Get<CommunitySettings>(CommunitySettings.Key);
            chkAdmin.Checked = cdo.SuperAdminRun;
            chkSide.Checked = cdo.SideMenuOpenByDefault.Contains("True");
            txtTest.Text = cdo.TestServer;
            flwPlugins.Controls.Add(new AccessControl());
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            CommunitySettings cdo = CustomDataObject.Get<CommunitySettings>(CommunitySettings.Key);
            cdo.SideMenuOpenByDefault = chkSide.Checked ? "True" : "False";
            cdo.SuperAdminRun = chkAdmin.Checked;
            cdo.TestServer = txtTest.Text;
            CustomDataObject.Save<CommunitySettings>(CommunitySettings.Key, cdo);
            MessageBox.Show($" Saved");
        }
    }
}
