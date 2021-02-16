namespace CommunityPlugin.Config
{
    partial class ExportServicePlugin_Config
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
            this.btnSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFieldID = new System.Windows.Forms.TextBox();
            this.cmbService = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkForms = new System.Windows.Forms.CheckedListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(266, 222);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Export Field ID";
            // 
            // txtFieldID
            // 
            this.txtFieldID.Location = new System.Drawing.Point(94, 36);
            this.txtFieldID.Name = "txtFieldID";
            this.txtFieldID.Size = new System.Drawing.Size(328, 20);
            this.txtFieldID.TabIndex = 2;
            // 
            // cmbService
            // 
            this.cmbService.FormattingEnabled = true;
            this.cmbService.Location = new System.Drawing.Point(94, 12);
            this.cmbService.Name = "cmbService";
            this.cmbService.Size = new System.Drawing.Size(328, 21);
            this.cmbService.TabIndex = 3;
            this.cmbService.SelectedIndexChanged += new System.EventHandler(this.cmbService_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "GSE Service";
            // 
            // chkForms
            // 
            this.chkForms.FormattingEnabled = true;
            this.chkForms.Location = new System.Drawing.Point(94, 62);
            this.chkForms.Name = "chkForms";
            this.chkForms.Size = new System.Drawing.Size(328, 154);
            this.chkForms.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Export Form";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(347, 222);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ExportServicePlugin_Config
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 252);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chkForms);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbService);
            this.Controls.Add(this.txtFieldID);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Name = "ExportServicePlugin_Config";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ExportServicePlugin_Config";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFieldID;
        private System.Windows.Forms.ComboBox cmbService;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckedListBox chkForms;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCancel;
    }
}