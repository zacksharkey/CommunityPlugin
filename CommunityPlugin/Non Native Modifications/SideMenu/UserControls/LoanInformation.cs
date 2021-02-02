using CommunityPlugin.Objects;
using CommunityPlugin.Objects.CustomDataObjects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using CommunityPlugin.Objects.Models;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Loans;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications.SideMenu.UserControls
{
    public class LoanInformation : LoanMenuControl, IFieldChange
    {
        private System.Windows.Forms.DataGridView dataGridView1;

        private Dictionary<string, string> Fields { get; set; }
        private Dictionary<string, EncompassFieldInfo> InfoLines => Fields.ToDictionary(x => x.Key.ToString(), x => new EncompassFieldInfo(x.Key, x.Value), StringComparer.OrdinalIgnoreCase);
        public override bool CanRun()
        {
            return PluginAccess.CheckAccess(nameof(LoanInformation));
        }

        public override bool CanShow()
        {
            return CanRun();
        }
        public LoanInformation()
        {
            InitializeComponent();
            CommunitySettings cdo = CustomDataObject.Get<CommunitySettings>(CommunitySettings.Key);
            //Fields = cdo.LoanInformation == null  || cdo.LoanInformation.Count.Equals(0) ? new Dictionary<string, string>() : cdo.LoanInformation.ContainsKey(EncompassHelper.LastPersona) ? cdo.LoanInformation[EncompassHelper.LastPersona] : cdo.LoanInformation["Default"];
            RefreshInfo();
        }

        private void RefreshInfo()
        {
            this.Name = nameof(LoanInformation);
            this.Size = new System.Drawing.Size(320, (InfoLines.Count + 1) * 21);
            this.dataGridView1.DataSource = (object)this.InfoLines.Values.ToList<EncompassFieldInfo>();
            this.dataGridView1.Dock = DockStyle.Fill;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Columns[0].ReadOnly = true;
            this.dataGridView1.BackgroundColor = this.dataGridView1.Parent.BackColor;
            this.dataGridView1.BorderStyle = BorderStyle.None;
            this.dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
            this.dataGridView1.DataBindingComplete += DataGridView1_DataBindingComplete;
        }

        private void DataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.BackColor = Color.Gray;
            style.ForeColor = Color.Black;

            if (this.dataGridView1.Rows.Count > 0)
            {
                this.dataGridView1.Rows[0].ReadOnly = true;
                this.dataGridView1.Rows[0].DefaultCellStyle = style;
            }

            this.dataGridView1.Refresh();
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView grid = sender as DataGridView;
                if (grid == null || e.RowIndex < 1)
                {
                    Refresh();
                    return;
                }

                string fieldID = grid.Rows[e.RowIndex].Cells[0].Value.ToString().Split('[')[1].Replace("]", string.Empty).Trim();
                EncompassApplication.CurrentLoan.Fields[fieldID].Value = grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            }
            catch (Exception ex)
            {
                Logger.HandleError(ex, nameof(LoanInformation), (object)null);
            }
        }


        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(150, 150);
            this.dataGridView1.TabIndex = 0;
            // 
            // LoanInformation
            // 
            this.Controls.Add(this.dataGridView1);
            this.Name = "LoanInformation";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }
        public override void FieldChanged(object sender, FieldChangeEventArgs e)
        {
            if (this.Fields.ContainsKey(e.FieldID))
                this.RefreshInfo();
        }
    }
}
