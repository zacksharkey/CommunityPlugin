using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Interface;
using EllieMae.Encompass.Automation;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CommunityPlugin.Non_Native_Modifications
{
    public class PrintFormToEfolder : Plugin, ILoanOpened
    {
        public override bool Authorized()
        {
            return PluginAccess.CheckAccess(nameof(PrintFormToEfolder));
        }

        public override void LoanOpened(object sender, EventArgs e)
        {
            //Add Button
            Button b = new Button();
            FormWrapper.EncompassForm.Controls.Add(b);
            b.Click += B_Click;
        }

        private void B_Click(object sender, EventArgs e)
        {
            (EncompassApplication.Screens[EncompassScreen.Loans] as LoansScreen).Print(((EncompassApplication.Screens[EncompassScreen.Loans] as LoansScreen).CurrentForm.Name));
           
        }

        private void Print(EllieMae.Encompass.Forms.Form f)
        {
            //EncompassApplication.Session.ServerEvents.
            //Rectangle bounds = this.Bounds;
            //using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            //{
            //    using (Graphics g = Graphics.FromImage(bitmap))
            //    {
            //        g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
            //    }
            //    bitmap.Save("C://test.jpg", ImageFormat.Jpeg);
            //}
        }
    }
}
