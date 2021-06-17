using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Args;
using CommunityPlugin.Objects.Interface;
using EllieMae.EMLite.eFolder;
using EllieMae.EMLite.eFolder.Documents;
using EllieMae.EMLite.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications
{
    public class SantiagoGridExample: Plugin, INativeFormLoaded
    {
        private GridView DocumentGrid;
        public override void NativeFormLoaded(object sender, FormOpenedArgs e)
        {
            if(e.OpenForm.Name.Equals("eFolderDialog"))
            {
                eFolderDialog eFolder = e.OpenForm as eFolderDialog;
                DocumentGrid = e.OpenForm.Controls.Find("gvDocuments", true)[0] as GridView;
                DocumentGrid.HeaderClick += DocumentGrid_HeaderClick;
                RefreshColumn();
            }
        }

        private void DocumentGrid_HeaderClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            RefreshColumn(false);
        }

        private void ContextMenuStrip_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("test");
        }

        private void Items_Change(object sender, EventArgs e)
        {
            RefreshColumn();
        }

        private void RefreshColumn(bool Add = true)
        {
            if (DocumentGrid != null)
            {
                GVColumn aiColumn = DocumentGrid.Columns.FirstOrDefault(x => x.Name.Equals("AI"));
                if (aiColumn != null)
                    DocumentGrid.Columns.Remove(aiColumn);
                aiColumn = new GVColumn("AI");
                if (Add)
                {
                    DocumentGrid.Columns.Add(aiColumn); 
                    foreach (GVItem row in DocumentGrid.Items)
                    {
                        row.SubItems[aiColumn.Name].Text = "sdfsdfsdf";
                    }
                }
                else
                    DocumentGrid.Columns.Remove(aiColumn);

                
            }
        }
    }
}
