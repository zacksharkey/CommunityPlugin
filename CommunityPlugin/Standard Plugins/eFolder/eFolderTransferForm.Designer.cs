namespace CommunityPlugin.Standard_Plugins.eFolder
{
    partial class eFolderTransferForm
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
            this.clbBuckets = new System.Windows.Forms.CheckedListBox();
            this.btnNone = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnTransfer = new System.Windows.Forms.Button();
            this.txtLoanNumber = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblBorrower = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // clbBuckets
            // 
            this.clbBuckets.FormattingEnabled = true;
            this.clbBuckets.Location = new System.Drawing.Point(12, 52);
            this.clbBuckets.Name = "clbBuckets";
            this.clbBuckets.Size = new System.Drawing.Size(491, 394);
            this.clbBuckets.TabIndex = 0;
            // 
            // btnNone
            // 
            this.btnNone.Location = new System.Drawing.Point(12, 23);
            this.btnNone.Name = "btnNone";
            this.btnNone.Size = new System.Drawing.Size(43, 23);
            this.btnNone.TabIndex = 2;
            this.btnNone.Text = "None";
            this.btnNone.UseVisualStyleBackColor = true;
            this.btnNone.Click += new System.EventHandler(this.btnNone_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(428, 452);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnTransfer
            // 
            this.btnTransfer.Location = new System.Drawing.Point(347, 452);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(75, 23);
            this.btnTransfer.TabIndex = 4;
            this.btnTransfer.Text = "Transfer";
            this.btnTransfer.UseVisualStyleBackColor = true;
            this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
            // 
            // txtLoanNumber
            // 
            this.txtLoanNumber.Location = new System.Drawing.Point(247, 8);
            this.txtLoanNumber.Name = "txtLoanNumber";
            this.txtLoanNumber.Size = new System.Drawing.Size(160, 20);
            this.txtLoanNumber.TabIndex = 5;
            this.txtLoanNumber.TextChanged += new System.EventHandler(this.txtLoanNumber_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(413, 8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(92, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Get Documents";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(170, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Loan Number";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 457);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(40, 13);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "Status:";
            // 
            // lblBorrower
            // 
            this.lblBorrower.AutoSize = true;
            this.lblBorrower.Location = new System.Drawing.Point(186, 36);
            this.lblBorrower.Name = "lblBorrower";
            this.lblBorrower.Size = new System.Drawing.Size(55, 13);
            this.lblBorrower.TabIndex = 9;
            this.lblBorrower.Text = "Borrower: ";
            // 
            // eFolderTransferForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(510, 480);
            this.Controls.Add(this.lblBorrower);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtLoanNumber);
            this.Controls.Add(this.btnTransfer);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnNone);
            this.Controls.Add(this.clbBuckets);
            this.MaximumSize = new System.Drawing.Size(526, 519);
            this.MinimumSize = new System.Drawing.Size(526, 519);
            this.Name = "eFolderTransferForm";
            this.Text = "eFolder Transfer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox clbBuckets;
        private System.Windows.Forms.Button btnNone;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnTransfer;
        private System.Windows.Forms.TextBox txtLoanNumber;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblBorrower;
    }
}