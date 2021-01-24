
using CommunityPlugin.Objects.BaseClasses;
using CommunityPlugin.Objects.Enums;
using CommunityPlugin.Objects.Interface;
using CommunityPlugin.Objects.Models.Translation;
using EllieMae.Encompass.BusinessObjects.Users;
using EllieMae.Encompass.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CommunityPlugin.Objects.Helpers
{
    public class GlobalConfiguration
    {
        private static int customCommandTimeout = -1;
        private static bool? useMilliSecondsForLastModified = new bool?();
        private static IDictionary<Assembly, XConfiguration> __caches = (IDictionary<Assembly, XConfiguration>)new Dictionary<Assembly, XConfiguration>(16);
        internal const string ConfigKeyOfDebugMode = "DebugMode";
        internal const string ConfigKeyOfDebugMode2 = "DebugInfo";
        internal const string ConfigKeyOfDebugMode3 = "Debug";
        internal const string ConfigKeyOfEncompassServerUri = "EncompassServerUri";
        internal const string ConfigKeyOfEncompassServerUserId = "EncompassServerUserId";
        internal const string ConfigKeyOfEncompassServerPassword = "EncompassServerPassword";
        internal const string ConfigKeyOfEncompassServerIsOffline = "EncompassServerIsOffline";
        internal const string ConfigKeyOfSQLServerPassword = "SQLServerPassword";
        private static string configFileName;

        public static bool Debug
        {
            get
            {
                XConfiguration configuration = GlobalConfiguration.Configuration;
                string strA = configuration["DebugMode"];
                if (string.IsNullOrWhiteSpace(strA))
                    strA = configuration["DebugInfo"];
                if (string.IsNullOrWhiteSpace(strA))
                    strA = configuration[nameof(Debug)];
                if (string.IsNullOrWhiteSpace(strA))
                    strA = "false";
                return string.Compare(strA, "true", true) == 0;
            }
        }

        public static bool GetBoolean(string key)
        {
            return GlobalConfiguration.GetBoolean(key, false);
        }

        public static bool GetBoolean(string key, bool defaultValue)
        {
            string appSetting = GlobalConfiguration.AppSettings[key];
            if (string.IsNullOrWhiteSpace(appSetting))
                return defaultValue;
            return "true".Equals(appSetting, StringComparison.CurrentCultureIgnoreCase);
        }

        public static string[] GetListByKey(string key)
        {
            string appSetting = GlobalConfiguration.AppSettings[key];
            if (string.IsNullOrWhiteSpace(appSetting))
                return new string[0];
            return ((IEnumerable<string>)appSetting.Split(',')).Select<string, string>((Func<string, string>)(s => s.Trim())).ToArray<string>();
        }

        public static RunMode Mode { get; private set; }

        internal static int CustomCommandTimeout
        {
            get
            {
                if (GlobalConfiguration.customCommandTimeout == -1)
                {
                    int result = 0;
                    GlobalConfiguration.customCommandTimeout = int.TryParse(GlobalConfiguration.AppSettings["SQLCommandTimeout"], out result) ? result : 30;
                }
                return GlobalConfiguration.customCommandTimeout;
            }
        }

        internal static bool UseMilliSecondsForLastModified
        {
            get
            {
                if (!GlobalConfiguration.useMilliSecondsForLastModified.HasValue)
                    GlobalConfiguration.useMilliSecondsForLastModified = new bool?(GlobalConfiguration.GetBoolean(nameof(UseMilliSecondsForLastModified), false));
                return GlobalConfiguration.useMilliSecondsForLastModified.Value;
            }
        }

        public static Session CurrentSession { get; private set; }

        public static GlobalConfiguration.InnerAppSettings AppSettings
        {
            get
            {
                return new GlobalConfiguration.InnerAppSettings(GlobalConfiguration.Configuration);
            }
        }

        public static IDictionary<string, ConnectionStringSettings> Connections
        {
            get
            {
                return GlobalConfiguration.Configuration.Connections;
            }
        }

        private static XConfiguration Configuration
        {
            get
            {
                Assembly key = GlobalConfiguration.GetAssembly();
                if (key == (Assembly)null)
                    key = GlobalConfiguration.__caches.Keys.Single<Assembly>();
                if (key != (Assembly)null && GlobalConfiguration.__caches.ContainsKey(key))
                    return GlobalConfiguration.__caches[key];
                throw new ArgumentNullException("The global configuration can't be initialized");
            }
        }

        public static void Init(RunMode mode)
        {
            GlobalConfiguration.Init((object)(Session)null, mode, string.Empty);
        }

        public static void Init(RunMode mode, string configFileName)
        {
            GlobalConfiguration.Init((object)null, mode, configFileName);
        }

        public static void Init(object session, RunMode mode)
        {
            GlobalConfiguration.Init(session, mode, string.Empty);
        }

        public static void Init(object session, RunMode mode, string configFileName)
        {
            GlobalConfiguration.Mode = mode;
            GlobalConfiguration.configFileName = configFileName;
            Assembly assembly = GlobalConfiguration.GetAssembly();
            if (assembly == (Assembly)null)
                throw new Exception("Can't find calling assembly.");
            if (session != null && session is Session)
                GlobalConfiguration.CurrentSession = session as Session;
            if (GlobalConfiguration.__caches.ContainsKey(assembly))
                return;
            XConfiguration configuration = GlobalConfiguration.GetConfiguration(mode, assembly);
            bool flag = false;
            if (configuration != null)
                GlobalConfiguration.__caches.Add(assembly, configuration);
            if (session == null)
                return;
            User currentUser = GlobalConfiguration.CurrentSession.GetCurrentUser();
            if (GlobalConfiguration.Configuration.RequiredKeyPairs.Count <= 0 || !currentUser.Personas.Contains(GlobalConfiguration.CurrentSession.Users.Personas.SuperAdministrator))
                return;
            foreach (KeyValuePair<string, XConfiguration.RequiredKeySetting> requiredKeyPair in (IEnumerable<KeyValuePair<string, XConfiguration.RequiredKeySetting>>)GlobalConfiguration.Configuration.RequiredKeyPairs)
            {
                if (string.IsNullOrWhiteSpace(requiredKeyPair.Value.Value))
                {
                    flag = true;
                    break;
                }
            }
            if (!flag && string.Equals(GlobalConfiguration.AppSettings["BypassConfigurationScreen"], "true", StringComparison.OrdinalIgnoreCase))
                return;
            GlobalConfiguration.RequiredConfigKeysReview(GlobalConfiguration.Configuration.RequiredKeyPairs, string.IsNullOrWhiteSpace(configFileName) ? assembly.GetName().Name : configFileName);
           // FrmConfigurationManager.AlreadyShown = true;
        }

        public static void Reset()
        {
            //FrmConfigurationManager.AlreadyShown = false;
            Assembly assembly = GlobalConfiguration.GetAssembly();
            if (!(assembly != (Assembly)null) || !GlobalConfiguration.__caches.ContainsKey(assembly))
                return;
            GlobalConfiguration.__caches.Remove(assembly);
        }

        public static void ShowConfigUI(string asmName)
        {
            if (GlobalConfiguration.Configuration.RequiredKeyPairs.Count == 0)
            {
                int num = (int)MessageBox.Show("There are no required settings to show for " + asmName, "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            //else if (FrmConfigurationManager.AlreadyShown)
            //{
            //    FrmConfigurationManager.AlreadyShown = false;
            //}
            else
            {
                if (!string.Equals(GlobalConfiguration.AppSettings["BypassConfigurationScreen"], "true", StringComparison.OrdinalIgnoreCase))
                    return;
                GlobalConfiguration.RequiredConfigKeysReview(GlobalConfiguration.Configuration.RequiredKeyPairs, asmName);
            }
        }

        public static void Save()
        {
            GlobalConfiguration.Save((object)GlobalConfiguration.getRuntimeSession());
        }

        public static void Save(object session)
        {
            GlobalConfiguration.Save(session, false);
        }

        public static void Save(object session, bool ignoreLog)
        {
            Assembly assembly = GlobalConfiguration.GetAssembly();
            XConfiguration configuration = GlobalConfiguration.Configuration;
            string configurationFileName = GlobalConfiguration.GetConfigurationFileName(GlobalConfiguration.Mode, assembly);
            switch (GlobalConfiguration.Mode)
            {
                case RunMode.EncompassServer:
                    if (session == null)
                        session = (object)GlobalConfiguration.getRuntimeSession();
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        EllieMae.Encompass.BusinessObjects.DataObject data = new EllieMae.Encompass.BusinessObjects.DataObject();
                        configuration.Save((Stream)memoryStream);
                        memoryStream.Flush();
                        if (!ignoreLog)
                        {
                            try
                            {
                                //GlobalTracer.TraceVerboseFormat("The length of configuration file stream is:{0}.", (object)memoryStream.Length);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        data.Load(memoryStream.ToArray());
                        if (!ignoreLog)
                        {
                            try
                            {
                                //GlobalTracer.TraceVerboseFormat("The size of data object is:{0}.", (object)data.Size);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        if (data.Size == 0)
                        {
                            if (!ignoreLog)
                            {
                                try
                                {
                                    //GlobalTracer.TraceError("Failed to save configuration.");
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                            throw new Exception("Failed to save configuration.");
                        }
                        try
                        {
                            (session as Session).DataExchange.SaveCustomDataObject(configurationFileName, data);
                            break;
                        }
                        catch (Exception ex1)
                        {
                            if (ignoreLog)
                                break;
                            try
                            {
                                //GlobalTracer.TraceErrorFormat("Failed to save changes to configuration file, details:{0}", (object)ex1.Message);
                                break;
                            }
                            catch (Exception ex2)
                            {
                                break;
                            }
                        }
                    }
                case RunMode.WebServer:
                case RunMode.Client:
                    configuration.Save(configurationFileName);
                    break;
            }
        }

        public static XmlSettings GetXmlSettingsByKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            string appSetting = GlobalConfiguration.AppSettings[key];
            if (string.IsNullOrWhiteSpace(appSetting))
                throw new ArgumentException(string.Format("Can't find key in configuration file.", (object)key));
            return GlobalConfiguration.GetXmlSettings(appSetting);
        }

        public static XmlSettings GetXmlSettings(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));
            XElement parent;
            switch (GlobalConfiguration.Mode)
            {
                case RunMode.EncompassServer:
                    parent = GlobalConfiguration.getSettingsElementFromCustomData(fileName);
                    break;
                case RunMode.WebServer:
                    parent = XElement.Load(HttpContext.Current.Server.MapPath(fileName));
                    break;
                case RunMode.Client:
                    parent = !File.Exists(fileName) ? GlobalConfiguration.getSettingsElementFromCustomData(fileName) : XElement.Load(fileName);
                    break;
                case RunMode.Test:
                    fileName = string.Format("{0}\\{1}", (object)new DirectoryInfo(Environment.CurrentDirectory).FullName, (object)fileName);
                    parent = XElement.Load(fileName);
                    break;
                default:
                    throw new NotSupportedException(string.Format("Can't call GetXmlSettings under:{0} mode", (object)GlobalConfiguration.Mode));
            }
            if (parent == null)
                throw new FileNotFoundException("Can't find settings file", fileName);
            return new XmlSettings(parent);
        }

        private static XElement getSettingsElementFromCustomData(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (GlobalConfiguration.CurrentSession == null)
                throw new InvalidOperationException("Please initialize encompass session in the first!");
            EllieMae.Encompass.BusinessObjects.DataObject customDataObject = GlobalConfiguration.CurrentSession.DataExchange.GetCustomDataObject(fileName);
            if (customDataObject == null || customDataObject.Size == 0)
                return (XElement)null;
            using (Stream stream = customDataObject.OpenStream())
                return XElement.Load(stream);
        }

        public static Stream OpenFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (GlobalConfiguration.Mode == RunMode.EncompassServer)
                return GlobalConfiguration.CurrentSession.DataExchange.GetCustomDataObject(fileName).OpenStream();
            if (!File.Exists(fileName))
            {
                if (GlobalConfiguration.Mode == RunMode.Client)
                {
                    fileName = string.Format("{0}\\{1}", (object)Path.GetDirectoryName(Environment.CurrentDirectory), (object)fileName);
                }
                else
                {
                    if (GlobalConfiguration.Mode != RunMode.WebServer)
                        throw new NotSupportedException("Inviald run mode");
                    fileName = HttpContext.Current.Server.MapPath(fileName);
                }
            }
            return (Stream)File.Open(fileName, FileMode.Open);
        }

        private static XConfiguration GetConfiguration(RunMode runMode, Assembly assembly)
        {
            if (assembly == (Assembly)null)
                throw new ArgumentNullException(nameof(assembly));
            switch (runMode)
            {
                case RunMode.EncompassServer:
                    Session runtimeSession = GlobalConfiguration.getRuntimeSession();
                    if (runtimeSession == null)
                        return XConfiguration.Empty;
                    EllieMae.Encompass.BusinessObjects.DataObject customDataObject = runtimeSession.DataExchange.GetCustomDataObject(GlobalConfiguration.GetConfigurationFileName(runMode, assembly));
                    if (customDataObject == null || customDataObject.Size == 0)
                        return XConfiguration.Empty;
                    using (Stream stream = customDataObject.OpenStream())
                    {
                        try
                        {
                            return new XConfiguration(stream);
                        }
                        catch (Exception ex)
                        {
                            return XConfiguration.Empty;
                        }
                    }
                case RunMode.WebServer:
                case RunMode.Client:
                    return new XConfiguration(GlobalConfiguration.GetConfigurationFileName(runMode, assembly));
                case RunMode.Test:
                    string name = string.Empty;
                    foreach (string manifestResourceName in assembly.GetManifestResourceNames())
                    {
                        if (manifestResourceName.EndsWith(string.Format("{0}.config", (object)assembly.GetName().Name)))
                        {
                            name = manifestResourceName;
                            break;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        using (Stream manifestResourceStream = assembly.GetManifestResourceStream(name))
                            return new XConfiguration(manifestResourceStream);
                    }
                    else
                        break;
            }
            return (XConfiguration)null;
        }

        public static string GetConnectionString(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            string configValue = GlobalConfiguration.Connections[name].ConnectionString;
            if (string.IsNullOrWhiteSpace("connectionString"))
                throw new ArgumentException("Invaild connection key in configuration file.");
            foreach (string str in (IEnumerable<string>)GlobalConfiguration.getDefinedItem(configValue, true).Values)
            {
                string itemValueByMask = GlobalConfiguration.getItemValueByMask(str);
                configValue = configValue.Replace(str, itemValueByMask);
            }
            return configValue;
        }

        public static void SetConnectionString(
          string name,
          string connectionString,
          IDictionary<string, string> replacedPairs)
        {
            if (replacedPairs.Count > 0)
            {
                IDictionary<string, string> definedItem = GlobalConfiguration.getDefinedItem(connectionString, false);
                StringBuilder stringBuilder = new StringBuilder();
                foreach (string key in (IEnumerable<string>)definedItem.Keys)
                {
                    string str = definedItem[key];
                    if (replacedPairs.ContainsKey(key))
                    {
                        string replacedPair = replacedPairs[key];
                        GlobalConfiguration.AppSettings[GlobalConfiguration.getItemKeyByMask(replacedPair)] = str;
                        str = replacedPair;
                    }
                    if (stringBuilder.Length > 0)
                        stringBuilder.Append(";");
                    stringBuilder.AppendFormat("{0}={1}", (object)key, (object)str);
                }
                connectionString = stringBuilder.ToString();
            }
            GlobalConfiguration.Configuration.SetConnection(name, connectionString);
        }

        private static IDictionary<string, string> getDefinedItem(
          string configValue,
          bool onlyReplacement)
        {
            string[] strArray = configValue.Split(';');
            IDictionary<string, string> dictionary = (IDictionary<string, string>)new Dictionary<string, string>();
            for (int index = 0; index < strArray.Length; ++index)
            {
                string key = string.Empty;
                string str = string.Empty;
                int length = strArray[index].IndexOf('=');
                if (length > 0)
                {
                    key = strArray[index].Substring(0, length).Trim();
                    str = strArray[index].Substring(length + 1, strArray[index].Length - length - 1).Trim();
                }
                if (!string.IsNullOrWhiteSpace(key) && (!onlyReplacement || str.StartsWith("$") && str.EndsWith("$") && str.Length > 2))
                    dictionary.Add(key, str);
            }
            return dictionary;
        }

        public static Session CreateSessionByDefault()
        {
            string appSetting1 = GlobalConfiguration.AppSettings["EncompassServerUri"];
            string appSetting2 = GlobalConfiguration.AppSettings["EncompassServerUserId"];
            string appSetting3 = GlobalConfiguration.AppSettings["EncompassServerPassword"];
            bool boolean = GlobalConfiguration.GetBoolean("EncompassServerIsOffline");
            Session session = new Session();
            if (boolean)
                session.StartOffline(appSetting2, appSetting3);
            else
                session.Start(appSetting1, appSetting2, appSetting3);
            return session;
        }

        public static bool TestEncompassByDefault(bool allowChangePassword, out Session session)
        {
            return GlobalConfiguration.TestEncompass("EncompassServerUri", "EncompassServerUserId", "EncompassServerPassword", "EncompassServerIsOffline", allowChangePassword, out session);
        }

        public static bool TestEncompass(
          string keyOfServer,
          string keyOfUser,
          string keyOfPassword,
          string keyOfIsOffline,
          bool allowChangePassword,
          out Session session)
        {
            if (string.IsNullOrWhiteSpace(keyOfServer))
                throw new ArgumentNullException(nameof(keyOfServer));
            if (string.IsNullOrWhiteSpace(keyOfUser))
                throw new ArgumentNullException(nameof(keyOfUser));
            if (string.IsNullOrWhiteSpace(keyOfPassword))
                throw new ArgumentNullException(nameof(keyOfPassword));
            if (string.IsNullOrWhiteSpace(keyOfIsOffline))
                throw new ArgumentNullException(nameof(keyOfIsOffline));
            string appSetting1 = GlobalConfiguration.AppSettings[keyOfServer];
            string appSetting2 = GlobalConfiguration.AppSettings[keyOfUser];
            string appSetting3 = GlobalConfiguration.AppSettings[keyOfPassword];
            bool boolean = GlobalConfiguration.GetBoolean(keyOfIsOffline);
            session = new Session();
            try
            {
                if (boolean)
                    session.StartOffline(appSetting2, appSetting3);
                else
                    session.Start(appSetting1, appSetting2, appSetting3);
            }
            catch (LoginException ex)
            {
                string newPassword;
                if (!allowChangePassword || !GlobalConfiguration.ShowChangePassword(string.Format("Incorrect password for Encompass user '{0}'. Please re-enter the password.", (object)appSetting2), appSetting3, out newPassword))
                    return false;
                GlobalConfiguration.AppSettings[keyOfPassword] = newPassword;
                if (GlobalConfiguration.TestEncompass(keyOfServer, keyOfUser, keyOfPassword, keyOfIsOffline, true, out session))
                    GlobalConfiguration.Save();
            }
            return true;
        }

        private static Session getRuntimeSession()
        {
            return GlobalConfiguration.CurrentSession != null ? GlobalConfiguration.CurrentSession : (Session)null;
        }

        public static void EncryptKey(string key)
        {
            GlobalConfiguration.EncryptKey(key, "3");
        }

        public static void EncryptKey(string key, string arithmetic)
        {
            GlobalConfiguration.Configuration.EncyptKey(key, arithmetic);
        }

        public static void RemoveEncryptKey(string key)
        {
            GlobalConfiguration.Configuration.RemoveEncryptKey(key);
        }

        public static bool IsEncryptKey(string key)
        {
            return GlobalConfiguration.Configuration.IsEncryptKey(key);
        }

        public static bool TestConnection(string name, bool allowChangePassword)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("passwordKey");
            using (IDbClient dbClient = DbFactory.CreateDBClient(name))
            {
                try
                {
                    dbClient.Open();
                }
                catch (Exception ex)
                {
                    if (!ex.Message.ToLower().Contains("login"))
                        throw ex;
                    if (!allowChangePassword)
                        return false;
                    string connectionString1 = GlobalConfiguration.Connections[name].ConnectionString;
                    IDictionary<string, string> definedItem1 = GlobalConfiguration.getDefinedItem(connectionString1, true);
                    IDictionary<string, string> definedItem2 = GlobalConfiguration.getDefinedItem(connectionString1, false);
                    string[] passwordKeys = new string[2]
                    {
            "password",
            "pwd"
                    };
                    string oldPassword = definedItem2.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>)(kv => ((IEnumerable<string>)passwordKeys).Contains<string>(kv.Key.ToLower()))).Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>)(kv => GlobalConfiguration.getItemValueByMask(kv.Value))).SingleOrDefault<string>();
                    string[] uidKeys = new string[2] { "uid", "user" };
                    string newPassword;
                    if (!GlobalConfiguration.ShowChangePassword(string.Format("Incorrect password for database user '{0}'. Please re-enter the password.", (object)definedItem2.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>)(kv => ((IEnumerable<string>)uidKeys).Contains<string>(kv.Key.ToLower()))).Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>)(kv => GlobalConfiguration.getItemValueByMask(kv.Value))).SingleOrDefault<string>()), oldPassword, out newPassword))
                        return false;
                    foreach (string index in definedItem2.Keys.ToArray<string>())
                    {
                        if (((IEnumerable<string>)passwordKeys).Contains<string>(index.ToLower()))
                            definedItem2[index] = newPassword;
                    }
                    string connectionString2 = string.Join(";", definedItem2.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>)(kv => string.Format("{0}={1}", (object)kv.Key, (object)kv.Value))).ToArray<string>());
                    GlobalConfiguration.SetConnectionString(name, connectionString2, definedItem1);
                    if (GlobalConfiguration.TestConnection(name, true))
                        GlobalConfiguration.Save();
                }
            }
            return true;
        }

        private static string getItemKeyByMask(string maskName)
        {
            if (string.IsNullOrEmpty(maskName))
                throw new ArgumentNullException(nameof(maskName));
            if (!maskName.StartsWith("$") || !maskName.EndsWith("$") || maskName.Length <= 2)
                throw new ArgumentException("Invaild mask:" + maskName);
            return maskName.Substring(1, maskName.Length - 2);
        }

        private static string getItemValueByMask(string maskName)
        {
            if (string.IsNullOrWhiteSpace(maskName))
                throw new ArgumentNullException(nameof(maskName));
            return maskName.StartsWith("$") && maskName.EndsWith("$") && maskName.Length > 2 ? GlobalConfiguration.AppSettings[GlobalConfiguration.getItemKeyByMask(maskName)] : maskName;
        }

        private static bool ShowChangePassword(
          string title,
          string oldPassword,
          out string newPassword)
        {
            newPassword = string.Empty;
            //using (FrmChangePassword frmChangePassword = new FrmChangePassword())
            //{
            //    frmChangePassword.Title = title;
            //    frmChangePassword.OldPassword = oldPassword;
            //    if (frmChangePassword.ShowDialog() == DialogResult.OK)
            //    {
            //        newPassword = frmChangePassword.NewPassword;
            //        return true;
            //    }
            //}
            return false;
        }

        private static string GetConfigurationFileName(RunMode mode, Assembly assembly)
        {
            switch (mode)
            {
                case RunMode.EncompassServer:
                    return string.Format("{0}.config", !string.IsNullOrEmpty(GlobalConfiguration.configFileName) ? (object)GlobalConfiguration.configFileName : (object)assembly.GetName().Name);
                case RunMode.WebServer:
                    return string.Format("{0}\\web.config", (object)AppDomain.CurrentDomain.BaseDirectory);
                case RunMode.Client:
                    string path = !string.IsNullOrEmpty(GlobalConfiguration.configFileName) ? GlobalConfiguration.configFileName : string.Format("{0}\\{1}.exe.config", (object)Environment.CurrentDirectory, (object)assembly.GetName().Name);
                    if (!File.Exists(path))
                        path = string.Format("{0}\\{1}.exe.config", (object)new FileInfo(assembly.CodeBase.Replace("file:///", string.Empty)).Directory.FullName, (object)assembly.GetName().Name);
                    if (!File.Exists(path))
                    {
                        Console.WriteLine(path);
                        throw new FileNotFoundException("Can't found configuration file for current assembly.");
                    }
                    return path;
                default:
                    return string.Empty;
            }
        }

        public static bool RequiredConfigKeysReview(
          IDictionary<string, XConfiguration.RequiredKeySetting> RequiredKeys,
          string AssemblyName)
        {
            bool flag1 = false;
            bool flag2 = true;
            try
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static void SaveInternal(XConfiguration configuration, string configFile)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                EllieMae.Encompass.BusinessObjects.DataObject data = new EllieMae.Encompass.BusinessObjects.DataObject();
                configuration.Save((Stream)memoryStream);
                memoryStream.Flush();
                data.Load(memoryStream.ToArray());
                if (data.Size == 0)
                    throw new Exception("Failed to save configuration.");
                try
                {
                    GlobalConfiguration.CurrentSession.DataExchange.SaveCustomDataObject(configFile, data);
                }
                catch (Exception ex)
                {
                }
            }
        }

        internal static Assembly GetAssembly()
        {
            string[] IgnoreAssemblyNames = new string[4]
            {
        "mscorlib",
        "System",
        "System.Core",
        "Utility4"
            };
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            if (callingAssembly.GetName().Name != "Utility4")
                return callingAssembly;
            StackTrace stackTrace = new StackTrace();
            Assembly[] array = Enumerable.Range(1, stackTrace.FrameCount - 1).Where<int>((Func<int, bool>)(i => stackTrace.GetFrame(i).GetMethod().DeclaringType != (Type)null)).Select<int, Assembly>((Func<int, Assembly>)(i => stackTrace.GetFrame(i).GetMethod().DeclaringType.Assembly)).Where<Assembly>((Func<Assembly, bool>)(asm => !((IEnumerable<string>)IgnoreAssemblyNames).Contains<string>(asm.GetName().Name))).ToArray<Assembly>();
            if (array.Length > 0)
                return array[0];
            Assembly assembly = Enumerable.Range(1, stackTrace.FrameCount - 1).Where<int>((Func<int, bool>)(i => stackTrace.GetFrame(i).GetMethod().DeclaringType != (Type)null)).Select<int, Assembly>((Func<int, Assembly>)(i => stackTrace.GetFrame(i).GetMethod().DeclaringType.Assembly)).Where<Assembly>((Func<Assembly, bool>)(asm => asm.GetName().Name != "Utility4")).First<Assembly>();
            if (assembly == (Assembly)null)
            {
                assembly = GlobalConfiguration.__caches.Keys.Single<Assembly>();
                if (assembly == (Assembly)null)
                    assembly = typeof(GlobalConfiguration).Assembly;
            }
            return assembly;
        }

        public class InnerAppSettings
        {
            private XConfiguration Configuration { get; set; }

            public string this[string key]
            {
                get
                {
                    return this.Configuration[key];
                }
                set
                {
                    this.Configuration[key] = value;
                }
            }

            internal InnerAppSettings(XConfiguration configuration)
            {
                this.Configuration = configuration;
            }
        }
    }
}
