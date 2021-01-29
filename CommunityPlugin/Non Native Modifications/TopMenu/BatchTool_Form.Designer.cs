namespace CommunityPlugin.Non_Native_Modifications.TopMenu
{
    partial class BatchTool_Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnImport = new System.Windows.Forms.Button();
            this.btnReport = new System.Windows.Forms.Button();
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.txtCopy = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnUpdateColumn = new System.Windows.Forms.Button();
            this.chkOnlyChanged = new System.Windows.Forms.CheckBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.gpReportControls = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.gpReportControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(13, 18);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(148, 23);
            this.btnImport.TabIndex = 0;
            this.btnImport.Text = "Import Batch File";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnReport
            // 
            this.btnReport.Location = new System.Drawing.Point(191, 18);
            this.btnReport.Name = "btnReport";
            this.btnReport.Size = new System.Drawing.Size(148, 23);
            this.btnReport.TabIndex = 1;
            this.btnReport.Text = "Use Selected Report";
            this.btnReport.UseVisualStyleBackColor = true;
            this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
            // 
            // dgvData
            // 
            this.dgvData.AllowUserToAddRows = false;
            this.dgvData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvData.Location = new System.Drawing.Point(12, 54);
            this.dgvData.MaximumSize = new System.Drawing.Size(1212, 648);
            this.dgvData.MinimumSize = new System.Drawing.Size(1212, 648);
            this.dgvData.Name = "dgvData";
            this.dgvData.RowHeadersVisible = false;
            this.dgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvData.Size = new System.Drawing.Size(1212, 648);
            this.dgvData.TabIndex = 2;
            // 
            // txtCopy
            // 
            this.txtCopy.Location = new System.Drawing.Point(20, 17);
            this.txtCopy.Name = "txtCopy";
            this.txtCopy.Size = new System.Drawing.Size(180, 20);
            this.txtCopy.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 4;
            // 
            // btnUpdateColumn
            // 
            this.btnUpdateColumn.Location = new System.Drawing.Point(206, 15);
            this.btnUpdateColumn.Name = "btnUpdateColumn";
            this.btnUpdateColumn.Size = new System.Drawing.Size(148, 23);
            this.btnUpdateColumn.TabIndex = 5;
            this.btnUpdateColumn.Text = "Update Column Values";
            this.btnUpdateColumn.UseVisualStyleBackColor = true;
            this.btnUpdateColumn.Click += new System.EventHandler(this.btnUpdateColumn_Click);
            // 
            // chkOnlyChanged
            // 
            this.chkOnlyChanged.AutoSize = true;
            this.chkOnlyChanged.Location = new System.Drawing.Point(401, 19);
            this.chkOnlyChanged.Name = "chkOnlyChanged";
            this.chkOnlyChanged.Size = new System.Drawing.Size(131, 17);
            this.chkOnlyChanged.TabIndex = 6;
            this.chkOnlyChanged.Text = "Update Changed Only";
            this.chkOnlyChanged.UseVisualStyleBackColor = true;
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(1076, 19);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(148, 28);
            this.btnRun.TabIndex = 7;
            this.btnRun.Text = "Run Batch";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // gpReportControls
            // 
            this.gpReportControls.Controls.Add(this.txtCopy);
            this.gpReportControls.Controls.Add(this.btnUpdateColumn);
            this.gpReportControls.Controls.Add(this.chkOnlyChanged);
            this.gpReportControls.Location = new System.Drawing.Point(355, 1);
            this.gpReportControls.Name = "gpReportControls";
            this.gpReportControls.Size = new System.Drawing.Size(535, 47);
            this.gpReportControls.TabIndex = 8;
            this.gpReportControls.TabStop = false;
            this.gpReportControls.Text = "Report Controls";
            this.gpReportControls.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(167, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Or";
            // 
            // BatchTool_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1236, 711);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.gpReportControls);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvData);
            this.Controls.Add(this.btnReport);
            this.Controls.Add(this.btnImport);
            this.MaximumSize = new System.Drawing.Size(1252, 750);
            this.MinimumSize = new System.Drawing.Size(1252, 750);
            this.Name = "BatchTool_Form";
            this.Text = "Batch Tool";
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.gpReportControls.ResumeLayout(false);
            this.gpReportControls.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnReport;
        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.TextBox txtCopy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnUpdateColumn;
        private System.Windows.Forms.CheckBox chkOnlyChanged;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.GroupBox gpReportControls;
        private System.Windows.Forms.Label label2;
    }
}