using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using CommunityPlugin.Objects.Models;
using EllieMae.EMLite.Common.UI;
using EllieMae.EMLite.RemotingServices;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Loans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications.SideMenu.UserControls
{
    public class FieldLookup : LoanMenuControl
    {
        private FieldDescriptors StandardFields;
        private FieldDescriptors VirtualFields;
        private FieldDescriptors CustomFields;
        private TableLayoutPanel tableLayoutPanel1;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button btnSearch;
        private TextBox txtSearch;
        private DataGridView dgvResults;
        private Button btnGo;
        private List<SearchResultField> results;
        public override bool CanRun()
        {
            return PluginAccess.CheckAccess(nameof(FieldLookup));
        }

        public override bool CanShow()
        {
            return CanRun();
        }

        public FieldLookup()
        {
            InitializeComponent();
            this.Name = "Field Lookup Tool";
            StandardFields = EncompassApplication.Session.Loans.FieldDescriptors.StandardFields;
            VirtualFields = EncompassApplication.Session.Loans.FieldDescriptors.VirtualFields;
            CustomFields = EncompassApplication.Session.Loans.FieldDescriptors.CustomFields;
            Width = 320;
            dgvResults.RowHeadersVisible = false;
            dgvResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvResults.ReadOnly = true;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            results = new List<SearchResultField>();

            if(!string.IsNullOrEmpty(txtSearch.Text))
                results = SearchFields(txtSearch.Text);

            dgvResults.DataSource = results;
        }

        private List<SearchResultField> SearchFields(string Search)
        {
            List<FieldDescriptor> results = new List<FieldDescriptor>();
            results = StandardFields.Cast<FieldDescriptor>().Where(x => x.FieldID.Equals(Search, StringComparison.OrdinalIgnoreCase)).ToList();
            if (results.Count < 1)
            {
                results = VirtualFields.Cast<FieldDescriptor>().Where(x => x.FieldID.Equals(Search, StringComparison.OrdinalIgnoreCase)).ToList();
                if (results.Count < 1)
                {
                    results = CustomFields.Cast<FieldDescriptor>().Where(x => x.FieldID.Equals(Search, StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }
            
            if (results.Count < 1)
            {
                results.AddRange(StandardFields.Cast<FieldDescriptor>().Where(x => x.FieldID.Contains(Search.ToUpper())).ToList());
                results.AddRange(VirtualFields.Cast<FieldDescriptor>().Where(x => x.FieldID.Contains(Search.ToUpper())).ToList());
                results.AddRange(CustomFields.Cast<FieldDescriptor>().Where(x => x.FieldID.Contains(Search.ToUpper())).ToList());
            }
            if (results.Count < 1)
            {
                results.AddRange(StandardFields.Cast<FieldDescriptor>().Where(x => x.FieldID.Equals(Search, StringComparison.OrdinalIgnoreCase)).ToList());
                results.AddRange(VirtualFields.Cast<FieldDescriptor>().Where(x => x.FieldID.Equals(Search, StringComparison.OrdinalIgnoreCase)).ToList());
                results.AddRange(CustomFields.Cast<FieldDescriptor>().Where(x => x.FieldID.Equals(Search, StringComparison.OrdinalIgnoreCase)).ToList());
            }

            if (results.Count < 1)
            {
                results.AddRange(StandardFields.Cast<FieldDescriptor>().Where(x => x.FieldID.Equals(Search, StringComparison.OrdinalIgnoreCase) || x.Description.ToUpper().Contains(Search.ToUpper()) || EncompassHelper.Val(x.FieldID).ToUpper().Contains(Search.ToUpper())).ToList());
                results.AddRange(VirtualFields.Cast<FieldDescriptor>().Where(x => x.FieldID.Equals(Search, StringComparison.OrdinalIgnoreCase) || x.Description.ToUpper().Contains(Search.ToUpper()) || EncompassHelper.Val(x.FieldID).ToUpper().Contains(Search.ToUpper())).ToList());
                results.AddRange(CustomFields.Cast<FieldDescriptor>().Where(x => x.FieldID.Equals(Search, StringComparison.OrdinalIgnoreCase) || x.Description.ToUpper().Contains(Search.ToUpper()) || EncompassHelper.Val(x.FieldID).ToUpper().Contains(Search.ToUpper())).ToList());
            }

            return results.Select(x => new SearchResultField() { FieldID = x.FieldID, Description = x.Description, FormattedValue = EncompassHelper.Val(x.FieldID) }).ToList();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            string fieldID = txtSearch.Text;
            if (!string.IsNullOrEmpty(fieldID))
                Session.Application.GetService<ILoanEditor>().GoToField(fieldID, true);
        }

        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dgvResults = new System.Windows.Forms.DataGridView();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.dgvResults, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.58333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 85.41666F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(294, 240);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // dgvResults
            // 
            this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvResults.Location = new System.Drawing.Point(3, 37);
            this.dgvResults.Name = "dgvResults";
            this.dgvResults.Size = new System.Drawing.Size(288, 200);
            this.dgvResults.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnSearch);
            this.flowLayoutPanel1.Controls.Add(this.txtSearch);
            this.flowLayoutPanel1.Controls.Add(this.btnGo);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(288, 28);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(3, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(67, 23);
            this.btnSearch.TabIndex = 0;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(76, 3);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(154, 20);
            this.txtSearch.TabIndex = 1;
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(236, 3);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(48, 23);
            this.btnGo.TabIndex = 2;
            this.btnGo.Text = "GoTo";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // FieldLookup
            // 
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FieldLookup";
            this.Size = new System.Drawing.Size(294, 240);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}