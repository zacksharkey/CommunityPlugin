namespace CommunityPlugin.Non_Native_Modifications.TopMenu
{
    partial class LoanFolderMove_Form
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
            this.label2 = new System.Windows.Forms.Label();
            this.txtCalculation = new System.Windows.Forms.TextBox();
            this.chkActive = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.cmbMilestone = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.dgvFolders = new System.Windows.Forms.DataGridView();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSimulate = new System.Windows.Forms.Button();
            this.Order = new System.Windows.Forms.Label();
            this.txtOrder = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFolders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrder)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(383, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Calculation";
            // 
            // txtCalculation
            // 
            this.txtCalculation.Location = new System.Drawing.Point(448, 70);
            this.txtCalculation.Multiline = true;
            this.txtCalculation.Name = "txtCalculation";
            this.txtCalculation.Size = new System.Drawing.Size(305, 158);
            this.txtCalculation.TabIndex = 3;
            // 
            // chkActive
            // 
            this.chkActive.AutoSize = true;
            this.chkActive.Location = new System.Drawing.Point(449, 23);
            this.chkActive.Name = "chkActive";
            this.chkActive.Size = new System.Drawing.Size(15, 14);
            this.chkActive.TabIndex = 4;
            this.chkActive.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(405, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Active";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(598, 234);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cmbMilestone
            // 
            this.cmbMilestone.FormattingEnabled = true;
            this.cmbMilestone.Location = new System.Drawing.Point(449, 43);
            this.cmbMilestone.Name = "cmbMilestone";
            this.cmbMilestone.Size = new System.Drawing.Size(305, 21);
            this.cmbMilestone.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(390, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Milestone";
            // 
            // dgvFolders
            // 
            this.dgvFolders.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFolders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFolders.Location = new System.Drawing.Point(12, 12);
            this.dgvFolders.MaximumSize = new System.Drawing.Size(357, 245);
            this.dgvFolders.MinimumSize = new System.Drawing.Size(357, 245);
            this.dgvFolders.Name = "dgvFolders";
            this.dgvFolders.RowHeadersVisible = false;
            this.dgvFolders.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFolders.Size = new System.Drawing.Size(357, 245);
            this.dgvFolders.TabIndex = 9;
            this.dgvFolders.SelectionChanged += new System.EventHandler(this.dgvFolders_SelectionChanged);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(375, 176);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(67, 23);
            this.btnTest.TabIndex = 10;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(678, 234);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSimulate
            // 
            this.btnSimulate.Location = new System.Drawing.Point(375, 205);
            this.btnSimulate.Name = "btnSimulate";
            this.btnSimulate.Size = new System.Drawing.Size(67, 23);
            this.btnSimulate.TabIndex = 12;
            this.btnSimulate.Text = "Simulate";
            this.btnSimulate.UseVisualStyleBackColor = true;
            this.btnSimulate.Click += new System.EventHandler(this.btnSimulate_Click);
            // 
            // Order
            // 
            this.Order.AutoSize = true;
            this.Order.Location = new System.Drawing.Point(636, 23);
            this.Order.Name = "Order";
            this.Order.Size = new System.Drawing.Size(33, 13);
            this.Order.TabIndex = 13;
            this.Order.Text = "Order";
            // 
            // txtOrder
            // 
            this.txtOrder.Location = new System.Drawing.Point(679, 21);
            this.txtOrder.Name = "txtOrder";
            this.txtOrder.Size = new System.Drawing.Size(74, 20);
            this.txtOrder.TabIndex = 14;
            // 
            // LoanFolderMove_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 267);
            this.Controls.Add(this.txtOrder);
            this.Controls.Add(this.Order);
            this.Controls.Add(this.btnSimulate);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.dgvFolders);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbMilestone);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chkActive);
            this.Controls.Add(this.txtCalculation);
            this.Controls.Add(this.label2);
            this.MaximumSize = new System.Drawing.Size(786, 306);
            this.MinimumSize = new System.Drawing.Size(786, 306);
            this.Name = "LoanFolderMove_Form";
            this.Text = "LoanFolderMove_Form";
            ((System.ComponentModel.ISupportInitialize)(this.dgvFolders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrder)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCalculation;
        private System.Windows.Forms.CheckBox chkActive;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox cmbMilestone;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView dgvFolders;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSimulate;
        private System.Windows.Forms.Label Order;
        private System.Windows.Forms.NumericUpDown txtOrder;
    }
}