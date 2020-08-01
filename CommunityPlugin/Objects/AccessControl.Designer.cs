namespace CommunityPlugin.Objects
{
    partial class AccessControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblPlugin = new System.Windows.Forms.Label();
            this.chkAllAccess = new System.Windows.Forms.CheckBox();
            this.cbPersonas = new System.Windows.Forms.CheckedListBox();
            this.chkTest = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblPlugin
            // 
            this.lblPlugin.AutoSize = true;
            this.lblPlugin.Location = new System.Drawing.Point(16, 16);
            this.lblPlugin.Name = "lblPlugin";
            this.lblPlugin.Size = new System.Drawing.Size(75, 13);
            this.lblPlugin.TabIndex = 0;
            this.lblPlugin.Text = "NameOfPlugin";
            // 
            // chkAllAccess
            // 
            this.chkAllAccess.AutoSize = true;
            this.chkAllAccess.Location = new System.Drawing.Point(11, 46);
            this.chkAllAccess.Name = "chkAllAccess";
            this.chkAllAccess.Size = new System.Drawing.Size(75, 17);
            this.chkAllAccess.TabIndex = 1;
            this.chkAllAccess.Text = "All Access";
            this.chkAllAccess.UseVisualStyleBackColor = true;
            // 
            // cbPersonas
            // 
            this.cbPersonas.FormattingEnabled = true;
            this.cbPersonas.Location = new System.Drawing.Point(11, 91);
            this.cbPersonas.Name = "cbPersonas";
            this.cbPersonas.Size = new System.Drawing.Size(120, 79);
            this.cbPersonas.TabIndex = 2;
            // 
            // chkTest
            // 
            this.chkTest.AutoSize = true;
            this.chkTest.Location = new System.Drawing.Point(11, 69);
            this.chkTest.Name = "chkTest";
            this.chkTest.Size = new System.Drawing.Size(71, 17);
            this.chkTest.TabIndex = 3;
            this.chkTest.Text = "Only Test";
            this.chkTest.UseVisualStyleBackColor = true;
            // 
            // AccessControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkTest);
            this.Controls.Add(this.cbPersonas);
            this.Controls.Add(this.chkAllAccess);
            this.Controls.Add(this.lblPlugin);
            this.Name = "AccessControl";
            this.Size = new System.Drawing.Size(150, 182);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPlugin;
        private System.Windows.Forms.CheckBox chkAllAccess;
        private System.Windows.Forms.CheckedListBox cbPersonas;
        private System.Windows.Forms.CheckBox chkTest;
    }
}
