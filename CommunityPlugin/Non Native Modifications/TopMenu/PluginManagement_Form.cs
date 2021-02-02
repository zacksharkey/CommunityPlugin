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

            flwPlugins.Controls.Add(new AccessControl());
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
         
            MessageBox.Show($" Saved");
        }
    }
}
