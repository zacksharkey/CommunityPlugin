using EllieMae.EMLite.Common;
using EllieMae.EMLite.DataEngine;
using EllieMae.EMLite.LoanServices;
using EllieMae.EMLite.RemotingServices;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace CommunityPlugin.Objects.InternalEM
{
    public class ExportService
    {
        private static readonly TraceSwitch sw = Tracing.GetTraceSwitch(Tracing.SwImportExport);
        private const string className = "ExportService";
        private LoanDataMgr loanDataMgr;
        private IExportService exportService;
        private ServiceSetting serviceSetting;

        public ExportService(LoanDataMgr loanDataMgr, ServiceSetting serviceSetting)
        {
            this.loanDataMgr = loanDataMgr;
            this.serviceSetting = serviceSetting;
            this.exportService = this.initializeAssembly();
            if (this.exportService == null)
                return;
            this.exportService.Bam = (IBam)new Bam(loanDataMgr, (Sessions.Session)null);
        }

        public bool IsAccessible()
        {
            return this.exportService.IsAccessible();
        }

        public bool ProcessLoans(string[] loanGuids)
        {
            return this.exportService.ProcessLoans(loanGuids);
        }

        public bool ValidateLoan(string loanGuid)
        {
            return this.exportService.ValidateLoan(loanGuid);
        }

        public void ProcessLoan(string loanGuid)
        {
            this.exportService.ProcessLoan(loanGuid);
        }

        public bool ExportData(string[] loanGuids)
        {
            return this.exportService.ExportData(loanGuids);
        }

        private IExportService initializeAssembly()
        {
            if (this.serviceSetting == null)
                return (IExportService)null;
            string str1 = SystemSettings.EpassDataDir + this.serviceSetting.FilePath;
            Tracing.Log(ExportService.sw, TraceLevel.Verbose, nameof(ExportService), "Initialize Assembly: " + str1);
            if (!File.Exists(str1))
                throw new FileNotFoundException();
            string fullName1 = AssemblyName.GetAssemblyName(str1).FullName;
            Tracing.Log(ExportService.sw, TraceLevel.Verbose, nameof(ExportService), "Display Name: " + fullName1);
            Assembly assembly1 = (Assembly)null;
            foreach (Assembly assembly2 in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly2.FullName == fullName1)
                    assembly1 = assembly2;
            }
            if (assembly1 == (Assembly)null)
            {
                FileStream fileStream = File.OpenRead(str1);
                byte[] numArray = new byte[fileStream.Length];
                fileStream.Read(numArray, 0, numArray.Length);
                fileStream.Close();
                Tracing.Log(ExportService.sw, TraceLevel.Verbose, nameof(ExportService), "Loading Assembly");

                assembly1 = Assembly.Load(numArray);
            }
            string fullName2 = typeof(IExportService).FullName;
            string typeName = (string)null;
            try
            {
                foreach (Type type in assembly1.GetTypes())
                {
                    if (type.GetInterface(fullName2) != (Type)null)
                        typeName = type.FullName;
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                string str2 = "ReflectionTypeLoadException occured:\r\n" + ex.Message + "\r\n\r\n" + "Assembly Types:\r\n";
                foreach (Type type in ex.Types)
                {
                    if (!(type == (Type)null))
                        str2 = str2 + "Assembly: " + (object)type.Assembly + "\r\n" + "AssemblyQualifiedName: " + type.AssemblyQualifiedName + "\r\n" + "FullName: " + type.FullName + "\r\n" + "\r\n";
                }
                string str3 = str2 + "LoaderException messages:\r\n";
                foreach (Exception loaderException in ex.LoaderExceptions)
                {
                    if (loaderException != null)
                        str3 = str3 + loaderException.Message + "\r\n";
                }
                string str4 = str3 + "\r\n";
                Tracing.Log(ExportService.sw, TraceLevel.Error, nameof(ExportService), str4 + "\r\n");
                throw;
            }
            catch (Exception ex)
            {
                string msg = "Exception trying to create instance:\r\n" + ex.Message + "\r\n\r\nInnerException: \r\n" + (object)ex.InnerException;
                Tracing.Log(ExportService.sw, TraceLevel.Error, nameof(ExportService), msg);
                throw;
            }
            Tracing.Log(ExportService.sw, TraceLevel.Verbose, nameof(ExportService), "Creating Instance: " + typeName);
            return (IExportService)assembly1.CreateInstance(typeName);
        }
    }
}
