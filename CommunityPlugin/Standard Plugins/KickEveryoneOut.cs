
/*
 * 
 * This is a tool built by Nikolai https://www.encompdev.com/ Lots of great free resources.
 * 
 * 
 * */

using CommunityPlugin.Objects;
using CommunityPlugin.Objects.Helpers;
using CommunityPlugin.Objects.Interface;
using System;
using System.Diagnostics;
using System.IO;
using System.Timers;

namespace CommunityPlugin.Standard_Plugins
{
    public class KickEveryoneOut : Plugin, ILogin
    {
        private Timer Timer = null;
        private DateTime Started = DateTime.MinValue;
        public override void Login(object sender, EventArgs e)
        {
            if (!EncompassHelper.IsSuper && Timer == null)
            {
                ClearAutoSaveFiles();
                if (Timer != null)
                    return;
                try
                {
                    Started = DateTime.Now;
                    Timer = new Timer(10000);
                    Timer.Elapsed += Timer_Elapsed;
                    Timer.AutoReset = false;
                    Timer.Enabled = true;
                }
                catch (Exception ex)
                {
                    Logger.HandleError(ex, nameof(KickEveryoneOut));
                }
            }
        }

        private void ClearAutoSaveFiles()
        {
            string str = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Encompass\\Autosave");
            if (str.Length < 30 || str.IndexOf("Encompass\\Autosave", StringComparison.OrdinalIgnoreCase) < 10 || !Directory.Exists(str))
                return;
            DeleteAllFiles(str);
        }

        private void DeleteAllFiles(string Directory)
        {
            DirectoryInfo info = new DirectoryInfo(Directory);
            foreach (FileInfo file in info.GetFiles())
            {
                if (!file.Name.Equals(".") && !file.Name.Equals(".."))
                {
                    try
                    {
                        file.Attributes = FileAttributes.Normal;
                        file.Delete();
                    }
                    catch (Exception ex)
                    {
                        Logger.HandleError(ex, nameof(KickEveryoneOut));
                    }
                }
            }

            foreach (DirectoryInfo directory in info.GetDirectories())
            {
                if (!directory.Name.Equals(".") && !directory.Name.Equals(".."))
                {
                    try
                    {
                        directory.Attributes = FileAttributes.Normal;
                        DeleteAllFiles(directory.FullName);
                        directory.Delete();
                    }
                    catch (Exception ex)
                    {
                        Logger.HandleError(ex, nameof(KickEveryoneOut));
                    }
                }
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DateTime now = DateTime.Now;
            DateTime Cutoff = new DateTime(now.Year, now.Month, now.Day, 4, 0, 0);

            if (Started < Cutoff && now >= Cutoff)
            {
                try
                {
                    EncompassHelper.Loan.Close();
                }
                catch(Exception ex)
                {
                    Logger.HandleError(ex, nameof(KickEveryoneOut));
                }

                try
                {
                    Process p = Process.GetCurrentProcess();
                    p.Kill();
                }
                catch (Exception ex)
                {
                    Logger.HandleError(ex, nameof(KickEveryoneOut));
                }
            }
        }
    }
}
