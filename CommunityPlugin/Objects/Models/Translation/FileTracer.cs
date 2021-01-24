using CommunityPlugin.Objects.BaseClasses;
using CommunityPlugin.Objects.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace CommunityPlugin.Objects.Models.Translation
{
    public class FileTracer : Tracer
    {
        public const int DefaultLogLevel = 5;
        public const string ConfigKeyOfPopUpErrors = "PopUpErrors";
        private StreamWriter traceWriter;

        public ServerTracer ServerTracer { get; set; }

        public int MaxRetriesForFileLog { get; set; }

        public int MilliSecBetweenFileLogRetries { get; set; }

        protected virtual string TraceFile { get; set; }

        public bool AutoFlush { get; set; }

        public int LogLevel { get; set; }

        public bool Persistent { get; set; }

        public bool PopUpErrors { get; set; }

        public FileTracer(string traceFile)
          : this(traceFile, string.Empty)
        {
        }

        public FileTracer(string traceFile, string messageFormat)
        {
            if (string.IsNullOrWhiteSpace(traceFile))
                throw new ArgumentNullException(nameof(traceFile));
            this.LogLevel = 5;
            this.PopUpErrors = GlobalConfiguration.GetBoolean(nameof(PopUpErrors), false);
            int result1;
            if (!int.TryParse(GlobalConfiguration.AppSettings[nameof(MaxRetriesForFileLog)], out result1))
                result1 = 5;
            int result2;
            if (!int.TryParse(GlobalConfiguration.AppSettings[nameof(MilliSecBetweenFileLogRetries)], out result2))
                result2 = 5000;
            this.MaxRetriesForFileLog = result1;
            this.MilliSecBetweenFileLogRetries = result2;
            this.init(traceFile, messageFormat);
            this.ServerTracer = (ServerTracer)null;
        }

        private void init(string traceFile, string messageFormat)
        {
            if (string.IsNullOrEmpty(traceFile))
                throw new ArgumentException("Invalid name of trace file.");
            FileInfo fileInfo = new FileInfo(traceFile);
            if (!fileInfo.Exists)
            {
                if (!fileInfo.Directory.Exists)
                {
                    try
                    {
                        fileInfo.Directory.Create();
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            this.MessageFormat = messageFormat;
            this.TraceFile = traceFile;
            this.Persistent = false;
            this.AutoFlush = true;
        }

        public override void Log(TraceLevel level, string message, string className)
        {
            int retries = 1;
            if (level == TraceLevel.Error && this.PopUpErrors)
            {
                int num = (int)MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            if (this.ServerTracer != null)
            {
                try
                {
                    this.ServerTracer.Log(level, message, className);
                }
                catch (Exception ex)
                {
                }
            }
            if (level > (TraceLevel)this.LogLevel)
                return;
            this.WriteLogFile(string.Format("[{0}]{1}:{2}({3})", (object)this.GetLevelText(level), (object)DateTime.Now, (object)message, (object)className), retries);
        }

        private void WriteLogFile(string line, int retries)
        {
            try
            {
                if (this.Persistent)
                {
                    lock (this)
                    {
                        if (this.traceWriter == null)
                        {
                            this.traceWriter = new StreamWriter(this.TraceFile, true);
                            this.traceWriter.AutoFlush = this.AutoFlush;
                        }
                        this.traceWriter.WriteLine(line);
                    }
                }
                else
                {
                    lock (this)
                    {
                        using (FileStream fileStream = File.Open(this.TraceFile, FileMode.Append, FileAccess.Write, FileShare.Write))
                        {
                            using (StreamWriter streamWriter = new StreamWriter((Stream)fileStream))
                                streamWriter.WriteLine(line);
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(string.Format("The process cannot access the file '{0}' because it is being used by another process....({1}/{2} attempts)", (object)this.TraceFile, (object)retries, (object)this.MaxRetriesForFileLog));
                if (retries < this.MaxRetriesForFileLog)
                {
                    Console.WriteLine(string.Format(".... retry in {0} seconds...", (object)(this.MilliSecBetweenFileLogRetries / 1000)));
                    Thread.Sleep(this.MilliSecBetweenFileLogRetries);
                    this.WriteLogFile(line, retries + 1);
                }
                else
                {
                    Console.WriteLine("Maximum retries to write on the log file reached. Application ended.");
                    Process.GetCurrentProcess().Kill();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
                return;
            if (this.ServerTracer != null && this.ServerTracer != null)
            {
                this.ServerTracer.Dispose();
                this.ServerTracer = (ServerTracer)null;
            }
            if (this.traceWriter == null)
                return;
            this.traceWriter.Dispose();
            this.traceWriter = (StreamWriter)null;
        }
    }
}
