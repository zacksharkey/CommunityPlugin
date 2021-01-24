namespace CommunityPlugin.Standard_Plugins
{
    partial class LoanFolderMove_Simulate
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
            this.dgvProgress = new System.Windows.Forms.DataGridView();
            this.btnOkay = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProgress)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvProgress
            // 
            this.dgvProgress.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProgress.Location = new System.Drawing.Point(12, 23);
            this.dgvProgress.Name = "dgvProgress";
            this.dgvProgress.Size = new System.Drawing.Size(278, 345);
            this.dgvProgress.TabIndex = 0;
            // 
            // btnOkay
            // 
            this.btnOkay.Location = new System.Drawing.Point(215, 374);
            this.btnOkay.Name = "btnOkay";
            this.btnOkay.Size = new System.Drawing.Size(75, 23);
            this.btnOkay.TabIndex = 1;
            this.btnOkay.Text = "Okay";
            this.btnOkay.UseVisualStyleBackColor = true;
            this.btnOkay.Click += new System.EventHandler(this.btnOkay_Click);
            // 
            // LoanFolderMove_Simulate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 400);
            this.Controls.Add(this.btnOkay);
            this.Controls.Add(this.dgvProgress);
            this.Name = "LoanFolderMove_Simulate";
            this.Text = "LoanFolderMove_Simulate";
            ((System.ComponentModel.ISupportInitialize)(this.dgvProgress)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvProgress;
        private System.Windows.Forms.Button btnOkay;
    }
}