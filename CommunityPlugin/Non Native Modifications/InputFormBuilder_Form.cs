using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Args;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using CommunityPlugin.Objects.Models;
using EllieMae.EMLite.FormEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications
{
    public class InputFormBuilder_Form : Plugin, ILogin, INativeFormLoaded
    {

        public override void Login(object sender, EventArgs e)
        {
            return;


            HostWin f = new HostWin();
        }
    }
}
