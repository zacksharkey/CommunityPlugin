using EllieMae.Encompass.Automation;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace CommunityPlugin.Objects
{
    public partial class AccessControl : UserControl
    {
        public AccessControl(string Name, bool AllAccess, bool AllTest, List<string> Personas)
        {
            InitializeComponent();

            lblPlugin.Text = Name;
            chkAllAccess.Checked = AllAccess;
            chkTest.Checked = AllTest;
            cbPersonas.Items.AddRange(EncompassApplication.Session.Users.Personas.Cast<EllieMae.Encompass.BusinessObjects.Users.Persona>().Select(x=>x.Name).ToArray());
            for(int i = 0; i < cbPersonas.Items.Count;i++)
            {
                if(false)
                {
                    cbPersonas.SetItemChecked(i, true);
                }
            }
        }
    }
}
