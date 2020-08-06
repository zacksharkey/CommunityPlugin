using CommunityPlugin.Objects.Interface;
using CommunityPlugin.Objects.Models;
using EllieMae.EMLite.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications.TopMenu
{
    public partial class AnalysisTool_Form : Form
    {
        List<AnalysisBase> AnalysisClasses = new List<AnalysisBase>();
        AnalysisBase CurrentAnalysis;
        public AnalysisTool_Form()
        {
            InitializeComponent();
            InterfaceHelper i = new InterfaceHelper();
            AnalysisClasses.AddRange(i.GetAll(typeof(AnalysisBase)).Select(x=> Activator.CreateInstance(x)).Cast<AnalysisBase>().ToList());
            cmbFilter.Items.AddRange(AnalysisClasses.Select(x=>x.GetType().Name).ToArray());
            Tracing.Debug = true;

            backgroundWorker1.DoWork += BackgroundWorker1_DoWork;
            backgroundWorker1.RunWorkerAsync();
        }


        private void BackgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            pnlStatus.Visible = true;

            foreach(AnalysisBase baseClass in AnalysisClasses)
                baseClass.LoadCache();


            pnlStatus.Visible = false;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if(CurrentAnalysis != null)
            {
                UpdateGrid(CurrentAnalysis.Search(txtSearch.Text));
            }
        }

        private void UpdateGrid(AnalysisResult Analysis)
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = Analysis.Result;
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            AnalysisBase current = AnalysisClasses.Where(x => x.GetType().Name.Equals(cmbFilter.Text)).FirstOrDefault();
            if (current == null)
                return;

            CurrentAnalysis = current;
        }
    }
}
