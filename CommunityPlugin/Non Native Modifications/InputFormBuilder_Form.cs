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
        private HostWin f;
        public override bool Authorized()
        {
            return false;
        }

        public override void Login(object sender, EventArgs e)
        {
            TabControl tabs = FormWrapper.TabControl;
            TabPage p = new TabPage("Input Form Builder");
            f = new HostWin();
            f.TopLevel = false;
            f.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            f.Dock = DockStyle.Fill;
            f.Show();
            p.Controls.Add(f);
            tabs.TabPages.Add(p);

            OpenFileDialog o = (OpenFileDialog)Reflect.GetField("ofdBrowse", f);
            OpenFileDialog s = (OpenFileDialog)Reflect.GetField("sfdBrowse", f);
            ToolStripMenuItem save = (ToolStripMenuItem)Reflect.GetField("mnuFileSave", f);
            save.Click += Save_Click;
        }

        public override void NativeFormLoaded(object sender, FormOpenedArgs e)
        {
            Form form = e.OpenForm;
            string Name = form.Name;
            if(Name.Equals("FormsManagementDialog"))
            {
                Button delete = form.Controls.Find("btnDelete", true)[0] as Button;
                Button rename = form.Controls.Find("btnRename", true)[0] as Button;
                Button upload = form.Controls.Find("btnUpload", true)[0] as Button;
                Button download = form.Controls.Find("btnDownload", true)[0] as Button;

                delete.Click += Delete_Click;
                rename.Click += Rename_Click;
                upload.Click += Upload_Click;
                download.Click += Download_Click;

            }
            else if(Name.Equals("FormSaveAsDialog")) //title Save Encompass Form
            {

            }
            else if(Name.Equals("PackageExportWizard"))
            {

            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            new IFBLog() { Action = Objects.Enums.IFBAction.Save, UserID = EncompassHelper.User.ID, Timestamp = DateTime.Now, NewObject = null };
        }

        private void Download_Click(object sender, EventArgs e)
        {
            new IFBLog() { Action = Objects.Enums.IFBAction.Download, UserID = EncompassHelper.User.ID, Timestamp = DateTime.Now, NewObject = null };
        }

        private void Upload_Click(object sender, EventArgs e)
        {
            new IFBLog() { Action = Objects.Enums.IFBAction.Upload, UserID = EncompassHelper.User.ID, Timestamp = DateTime.Now, NewObject = null };
        }

        private void Rename_Click(object sender, EventArgs e)
        {
            new IFBLog() { Action = Objects.Enums.IFBAction.Rename, UserID = EncompassHelper.User.ID, Timestamp = DateTime.Now, NewObject = null };
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            new IFBLog() { Action = Objects.Enums.IFBAction.Delete, UserID = EncompassHelper.User.ID, Timestamp = DateTime.Now, NewObject = null };
        }
    }
}
