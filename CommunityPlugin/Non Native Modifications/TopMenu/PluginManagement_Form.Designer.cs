namespace CommunityPlugin.Non_Native_Modifications.TopMenu
{
    partial class PluginManagement_Form
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
            this.flwPlugins = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtTest = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkAdmin = new System.Windows.Forms.CheckBox();
            this.chkSide = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flwPlugins.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flwPlugins
            // 
            this.flwPlugins.AutoScroll = true;
            this.flwPlugins.Controls.Add(this.groupBox1);
            this.flwPlugins.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flwPlugins.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flwPlugins.Location = new System.Drawing.Point(0, 0);
            this.flwPlugins.Name = "flwPlugins";
            this.flwPlugins.Size = new System.Drawing.Size(296, 393);
            this.flwPlugins.TabIndex = 0;
            this.flwPlugins.WrapContents = false;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(202, 62);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtTest
            // 
            this.txtTest.Location = new System.Drawing.Point(70, 19);
            this.txtTest.Name = "txtTest";
            this.txtTest.Size = new System.Drawing.Size(196, 20);
            this.txtTest.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Test URL";
            // 
            // chkAdmin
            // 
            this.chkAdmin.AutoSize = true;
            this.chkAdmin.Location = new System.Drawing.Point(13, 45);
            this.chkAdmin.Name = "chkAdmin";
            this.chkAdmin.Size = new System.Drawing.Size(124, 17);
            this.chkAdmin.TabIndex = 3;
            this.chkAdmin.Text = "Super Admin Access";
            this.chkAdmin.UseVisualStyleBackColor = true;
            // 
            // chkSide
            // 
            this.chkSide.AutoSize = true;
            this.chkSide.Location = new System.Drawing.Point(13, 68);
            this.chkSide.Name = "chkSide";
            this.chkSide.Size = new System.Drawing.Size(103, 17);
            this.chkSide.TabIndex = 4;
            this.chkSide.Text = "Open SideMenu";
            this.chkSide.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkAdmin);
            this.groupBox1.Controls.Add(this.txtTest);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.chkSide);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(290, 95);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Global Settings";
            // 
            // PluginManagement_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(296, 393);
            this.Controls.Add(this.flwPlugins);
            this.Name = "PluginManagement_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Plugin Management";
            this.flwPlugins.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flwPlugins;
        private System.Windows.Forms.CheckBox chkAdmin;
        private System.Windows.Forms.CheckBox chkSide;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTest;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}