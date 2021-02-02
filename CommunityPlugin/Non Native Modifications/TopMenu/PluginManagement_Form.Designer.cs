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
            this.SuspendLayout();
            // 
            // flwPlugins
            // 
            this.flwPlugins.AutoScroll = true;
            this.flwPlugins.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flwPlugins.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flwPlugins.Location = new System.Drawing.Point(0, 0);
            this.flwPlugins.Name = "flwPlugins";
            this.flwPlugins.Size = new System.Drawing.Size(296, 393);
            this.flwPlugins.TabIndex = 0;
            this.flwPlugins.WrapContents = false;
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flwPlugins;
    }
}