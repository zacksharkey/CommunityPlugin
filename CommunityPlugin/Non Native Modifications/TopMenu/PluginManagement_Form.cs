using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Models;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu
{
    public partial class PluginManagement_Form : Form
    {
        public PluginManagement_Form()
        {
            InitializeComponent();
            CDO cdo = CDOHelper.CDO;
            chkAdmin.Checked = cdo.CommunitySettings.SuperAdminRun;
            chkSide.Checked = cdo.CommunitySettings.SideMenuOpenByDefault.Contains("True");
            txtTest.Text = cdo.CommunitySettings.TestServer;
            flwPlugins.Controls.Add(new AccessControl());
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            CDO cdo = CDOHelper.CDO;
            cdo.CommunitySettings.SideMenuOpenByDefault = chkSide.Checked ? "True" : "False";
            cdo.CommunitySettings.SuperAdminRun = chkAdmin.Checked;
            cdo.CommunitySettings.TestServer = txtTest.Text;
            CDOHelper.UpdateCDO(cdo);
            CDOHelper.UploadCDO();
            MessageBox.Show($" Saved");
        }
    }
}
