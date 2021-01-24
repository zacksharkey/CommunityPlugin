using CommunityPlugin.Objects.Enums;
using CommunityPlugin.Objects.Interface;
using CommunityPlugin.Objects.Models.Translation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CommunityPlugin.Objects.BaseClasses
{
    public class XConfiguration : ISettings
    {
        private static string[] AllowEncryptedItems = new string[2]
        {
      "appSettings",
      "connectionStrings"
        };
        private static string[] AllowRequiredKeyItems = new string[2]
        {
      "appSettings",
      "connectionStrings"
        };
        private const string DefaultCryptoKey = "0011Utility4EEFF";
        private XDocument _configDocument;
        private IList<string> _changedKeys;
        private IList<string> _encryptedKeys;
        private IDictionary<string, string> _appSettings;
        private IDictionary<string, XConfiguration.RequiredKeySetting> _requiredKeyPairs;
        private IDictionary<string, ConnectionStringSettings> _connectionSettings;
        private List<string> _changedConnections;

        public string this[string key]
        {
            get
            {
                return !this._appSettings.ContainsKey(key) ? string.Empty : this._appSettings[key];
            }
            set
            {
                if (!this._appSettings.ContainsKey(key))
                    this._appSettings.Add(key, value);
                else
                    this._appSettings[key] = value;
                if (this._changedKeys.Contains(key))
                    return;
                this._changedKeys.Add(key);
            }
        }

        public IDictionary<string, string> Settings
        {
            get
            {
                return this.AppSettings;
            }
        }

        public IDictionary<string, string> AppSettings
        {
            get
            {
                return this._appSettings;
            }
        }

        public IDictionary<string, XConfiguration.RequiredKeySetting> RequiredKeyPairs
        {
            get
            {
                return this._requiredKeyPairs;
            }
        }

        public IDictionary<string, ConnectionStringSettings> Connections
        {
            get
            {
                return this._connectionSettings;
            }
        }

        public void SetConnection(string key, string connectionString)
        {
            if (!this._connectionSettings.ContainsKey(key))
                this._connectionSettings.Add(key, new ConnectionStringSettings()
                {
                    ConnectionString = connectionString,
                    ProviderName = "System.Data.MsSql"
                });
            else
                this._connectionSettings[key].ConnectionString = connectionString;
            if (this._changedConnections.Contains(key))
                return;
            this._changedConnections.Add(key);
        }

        public static XConfiguration Empty
        {
            get
            {
                return new XConfiguration();
            }
        }

        public XConfiguration(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            try
            {
                this._configDocument = XDocument.Load(stream);
            }
            catch (Exception ex)
            {
                this._configDocument = this.createEmptyDoc();
            }
            this.init();
        }

        public XConfiguration(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (!File.Exists(fileName))
                throw new FileNotFoundException(string.Format("The configuratin file:{0} doesn't exist.", (object)fileName));
            try
            {
                this._configDocument = XDocument.Load(fileName);
            }
            catch (Exception ex)
            {
                this._configDocument = this.createEmptyDoc();
            }
            this.init();
        }

        private XConfiguration()
        {
            if (this._configDocument == null)
                this._configDocument = this.createEmptyDoc();
            this.init();
        }

        private XDocument createEmptyDoc()
        {
            return XDocument.Parse("<configuration><appSettings/></configuration>");
        }

        private void init()
        {
            if (this._configDocument == null)
                throw new Exception("The xml document is empty.");
            this._appSettings = (IDictionary<string, string>)this._configDocument.Document.XPathSelectElements("configuration/appSettings/add").ToDictionary<XElement, string, string>((Func<XElement, string>)(e => (string)e.Attribute((XName)"key")), (Func<XElement, string>)(e => (string)e.Attribute((XName)"value")));
            this._connectionSettings = (IDictionary<string, ConnectionStringSettings>)this._configDocument.Document.XPathSelectElements("configuration/connectionStrings/add").ToDictionary<XElement, string, ConnectionStringSettings>((Func<XElement, string>)(e => (string)e.Attribute((XName)"name")), (Func<XElement, ConnectionStringSettings>)(e => new ConnectionStringSettings((string)e.Attribute((XName)"name"), (string)e.Attribute((XName)"connectionString"), (string)e.Attribute((XName)"providerName"))));
            if (this._appSettings == null)
                this._appSettings = (IDictionary<string, string>)new Dictionary<string, string>();
            if (this._connectionSettings == null)
                this._connectionSettings = (IDictionary<string, ConnectionStringSettings>)new Dictionary<string, ConnectionStringSettings>(8);
            this.convertCryptedKeys(true);
            this._changedKeys = (IList<string>)new List<string>(this._appSettings.Count);
            this._changedConnections = new List<string>(this._connectionSettings.Count);
            this._requiredKeyPairs = (IDictionary<string, XConfiguration.RequiredKeySetting>)new Dictionary<string, XConfiguration.RequiredKeySetting>(this._appSettings.Count);
            this.validateRequiredSettings();
        }

        public void EncyptKey(string key)
        {
            this.EncyptKey(key, "3");
        }

        public void EncyptKey(string key, string arithmetic)
        {
            if (this._configDocument.Document.XPathSelectElements("configuration/configSections/section").SingleOrDefault<XElement>() == null)
                this._configDocument.Document.XPathSelectElements("configuration/appSettings").SingleOrDefault<XElement>().AddBeforeSelf((object)new XElement((XName)"configSections", (object)new XElement((XName)"section", new object[2]
                {
          (object) new XAttribute((XName) "name", (object) "requiredSettings"),
          (object) new XAttribute((XName) "type", (object) "Utility4.Configuration.EncryptedSectionHandler")
                })));
            string expression = "configuration/encryptedSettings";
            XElement node = this._configDocument.Document.XPathSelectElements(expression).SingleOrDefault<XElement>();
            if (node == null)
            {
                this._configDocument.Document.XPathSelectElements("configuration").SingleOrDefault<XElement>().Add((object)new XElement((XName)"encryptedSettings"));
                node = this._configDocument.Document.XPathSelectElements(expression).SingleOrDefault<XElement>();
            }
            string str = string.Format("appSettings/@{0}", (object)key);
            if (node.XPathSelectElement(string.Format("add[@key='{0}']", (object)str)) != null)
                return;
            XElement xelement = new XElement((XName)"add");
            xelement.Add((object)new XAttribute((XName)nameof(key), (object)str));
            xelement.Add((object)new XAttribute((XName)nameof(arithmetic), (object)arithmetic));
            node.Add((object)xelement);
        }

        public void RemoveEncryptKey(string key)
        {
            string expression = "configuration/encryptedSettings";
            XElement node = this._configDocument.Document.XPathSelectElements(expression).SingleOrDefault<XElement>();
            if (node == null)
            {
                this._configDocument.Document.XPathSelectElements("configuration").SingleOrDefault<XElement>().Add((object)new XElement((XName)"encryptedSettings"));
                node = this._configDocument.Document.XPathSelectElements(expression).SingleOrDefault<XElement>();
            }
            string str = string.Format("appSettings/@{0}", (object)key);
            node.XPathSelectElement(string.Format("add[@key='{0}']", (object)str))?.Remove();
            if (node.HasElements)
                return;
            this._configDocument.Document.XPathSelectElements("configuration/configSections/section").SingleOrDefault<XElement>().Remove();
        }

        public bool IsEncryptKey(string key)
        {
            string expression = "configuration/encryptedSettings";
            XElement node = this._configDocument.Document.XPathSelectElements(expression).SingleOrDefault<XElement>();
            if (node == null)
            {
                this._configDocument.Document.XPathSelectElements("configuration").SingleOrDefault<XElement>().Add((object)new XElement((XName)"encryptedSettings"));
                node = this._configDocument.Document.XPathSelectElements(expression).SingleOrDefault<XElement>();
            }
            string str = string.Format("appSettings/@{0}", (object)key);
            return node.XPathSelectElement(string.Format("add[@key='{0}']", (object)str)) != null;
        }

        private byte[] getCryptoKey()
        {
            string s = string.Empty;
            XElement xelement = this._configDocument.Document.XPathSelectElements("configuration/encryptedSettings/add[@key='Key']").SingleOrDefault<XElement>();
            if (xelement != null)
                s = (string)xelement.Attribute((XName)"value");
            if (string.IsNullOrWhiteSpace(s))
                s = "0011Utility4EEFF";
            return Encoding.UTF8.GetBytes(s);
        }

        private void convertCryptedKeys(bool decryptOrEncrypt)
        {
            this._encryptedKeys = (IList<string>)new List<string>(this._appSettings.Count);
            foreach (XElement xelement in this._configDocument.Document.XPathSelectElements("configuration/encryptedSettings/add").Where<XElement>((Func<XElement, bool>)(xe =>
            {
                foreach (string allowEncryptedItem in XConfiguration.AllowEncryptedItems)
                {
                    if (((string)xe.Attribute((XName)"key")).StartsWith(allowEncryptedItem))
                        return true;
                }
                return false;
            })).ToArray<XElement>())
            {
                foreach (string allowEncryptedItem in XConfiguration.AllowEncryptedItems)
                {
                    if (((string)xelement.Attribute((XName)"key")).StartsWith(allowEncryptedItem))
                    {
                        string[] strArray = ((string)xelement.Attribute((XName)"key")).Split('/');
                        string index = strArray[strArray.Length - 1];
                        if (index.StartsWith("@"))
                            index = index.Substring(1, index.Length - 1);
                        string arithmetic = (string)xelement.Attribute((XName)"arithmetic");
                        if (string.IsNullOrWhiteSpace(arithmetic))
                            arithmetic = string.Empty;
                        switch (allowEncryptedItem)
                        {
                            case "appSettings":
                                if (decryptOrEncrypt)
                                {
                                    this._appSettings[index] = this.decryptValue(this._appSettings[index], arithmetic);
                                    if (!this._encryptedKeys.Contains(index))
                                    {
                                        this._encryptedKeys.Add(index);
                                        continue;
                                    }
                                    continue;
                                }
                                this._appSettings[index] = this.encryptValue(this._appSettings[index], arithmetic);
                                continue;
                            case "connectionStrings":
                                string key = strArray.Length > 1 ? strArray[1] : string.Empty;
                                if (!this._connectionSettings.ContainsKey(key))
                                    throw new ArgumentException(string.Format("Invalid database connection name:{0}", (object)key));
                                switch (index)
                                {
                                    case "connectionString":
                                        if (decryptOrEncrypt)
                                        {
                                            this._connectionSettings[key].ConnectionString = this.decryptValue(this._connectionSettings[key].ConnectionString, arithmetic);
                                            if (!this._encryptedKeys.Contains(index))
                                            {
                                                this._encryptedKeys.Add(index);
                                                continue;
                                            }
                                            continue;
                                        }
                                        this._connectionSettings[key].ConnectionString = this.encryptValue(this._connectionSettings[key].ConnectionString, arithmetic);
                                        continue;
                                    case "providerName":
                                        if (decryptOrEncrypt)
                                        {
                                            this._connectionSettings[key].ProviderName = this.decryptValue(this._connectionSettings[key].ProviderName, arithmetic);
                                            if (!this._encryptedKeys.Contains(index))
                                            {
                                                this._encryptedKeys.Add(index);
                                                continue;
                                            }
                                            continue;
                                        }
                                        this._connectionSettings[key].ProviderName = this.encryptValue(this._connectionSettings[key].ProviderName, arithmetic);
                                        continue;
                                    default:
                                        continue;
                                }
                            default:
                                continue;
                        }
                    }
                }
            }
        }

        private SymmetricCrypto createCrypto(string arithmetic)
        {
            SymmetricAlgorithmType result;
            if (string.IsNullOrWhiteSpace(arithmetic))
                result = SymmetricAlgorithmType.TripleDES;
            else if (!Enum.TryParse<SymmetricAlgorithmType>(arithmetic, out result))
                throw new CryptographicException(string.Format("Invalid arithmetic:{0}", (object)arithmetic));
            return SymmetricCrypto.Create(this.getCryptoKey(), result);
        }

        private string decryptValue(string itemValue, string arithmetic)
        {
            return this.createCrypto(arithmetic).Decrypt(itemValue);
        }

        private string encryptValue(string itemValue, string arithmetic)
        {
            return this.createCrypto(arithmetic).Encrypt(itemValue);
        }

        private bool parseEncryptedAppKey(string path, out string key)
        {
            key = string.Empty;
            int num = path.LastIndexOf('@');
            if (num < 0)
                return false;
            key = path.Substring(num + 1);
            return true;
        }

        public bool GetBoolSetting(string key)
        {
            return this.GetBoolSetting(key, false);
        }

        public bool GetBoolSetting(string key, bool defaultValue)
        {
            return string.Compare(this.GetSetting(key, defaultValue ? "true" : "false"), "true", true) == 0;
        }

        public string GetSetting(string key)
        {
            return this.GetSetting(key, string.Empty);
        }

        public string GetSetting(string key, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            if (this.Settings == null)
                throw new ArgumentNullException("settings");
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            string str = string.Empty;
            if (this.Settings.ContainsKey(key))
                str = this.Settings[key];
            return !string.IsNullOrWhiteSpace(str) ? str : defaultValue;
        }

        public void Save(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (!File.Exists(fileName))
                throw new FileNotFoundException(string.Format("Can't find file: {0}", (object)fileName));
            this.Save((object)fileName);
        }

        public void Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            this.Save((object)stream);
        }

        public void Merge(XConfiguration xc)
        {
            if (xc == null)
                throw new ArgumentNullException(nameof(xc));
            foreach (string key in (IEnumerable<string>)xc.AppSettings.Keys)
            {
                if (this.AppSettings.ContainsKey(key))
                    this.AppSettings[key] = xc.AppSettings[key];
                else
                    this.AppSettings.Add(key, xc.AppSettings[key]);
            }
            foreach (string key in (IEnumerable<string>)xc.Connections.Keys)
            {
                if (this.Connections.ContainsKey(key))
                    this.Connections[key] = xc.Connections[key];
                else
                    this.Connections.Add(key, xc.Connections[key]);
            }
        }

        protected virtual void Save(object target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            this.convertCryptedKeys(false);
            try
            {
                this.SaveValues();
                if (target is Stream)
                    this._configDocument.Save(target as Stream);
                else
                    this._configDocument.Save(string.Format("{0}", target));
            }
            finally
            {
                this.convertCryptedKeys(true);
            }
        }

        private void SaveValues()
        {
            foreach (string changedKey in (IEnumerable<string>)this._changedKeys)
            {
                string appSetting = this._appSettings[changedKey];
                XElement xelement = this._configDocument.Document.XPathSelectElement(string.Format("configuration/appSettings/add[@key='{0}']", (object)changedKey));
                if (xelement != null)
                    xelement.Attribute((XName)"value").Value = appSetting;
                else
                    this._configDocument.Document.Element((XName)"configuration").Element((XName)"appSettings").Add((object)new XElement((XName)"add", new object[2]
                    {
            (object) new XAttribute((XName) "key", (object) changedKey),
            (object) new XAttribute((XName) "value", (object) appSetting)
                    }));
            }
            this._changedKeys.Clear();
            foreach (string changedConnection in this._changedConnections)
            {
                string connectionString = this._connectionSettings[changedConnection].ConnectionString;
                string providerName = this._connectionSettings[changedConnection].ProviderName;
                XElement xelement = this._configDocument.Document.XPathSelectElement(string.Format("configuration/connectionStrings/add[@name='{0}']", (object)changedConnection));
                if (xelement != null)
                {
                    xelement.Attribute((XName)"connectionString").Value = connectionString;
                    xelement.Attribute((XName)"providerName").Value = providerName;
                }
                else
                    this._configDocument.Document.Element((XName)"configuration").Element((XName)"connectionStrings").Add((object)new XElement((XName)"add", new object[3]
                    {
            (object) new XAttribute((XName) "name", (object) changedConnection),
            (object) new XAttribute((XName) "connectionString", (object) connectionString),
            (object) new XAttribute((XName) "providerName", (object) providerName)
                    }));
            }
            this._changedConnections.Clear();
        }

        private void validateRequiredSettings()
        {
            foreach (XElement xelement in this._configDocument.Document.XPathSelectElements("configuration/requiredSettings/add").Where<XElement>((Func<XElement, bool>)(xe =>
            {
                foreach (string allowRequiredKeyItem in XConfiguration.AllowRequiredKeyItems)
                {
                    if (((string)xe.Attribute((XName)"key")).StartsWith(allowRequiredKeyItem))
                        return true;
                }
                return false;
            })).ToArray<XElement>())
            {
                foreach (string allowRequiredKeyItem in XConfiguration.AllowRequiredKeyItems)
                {
                    if (((string)xelement.Attribute((XName)"key")).StartsWith(allowRequiredKeyItem))
                    {
                        string[] strArray = ((string)xelement.Attribute((XName)"key")).Split('/');
                        string key = strArray[strArray.Length - 1];
                        if (key.StartsWith("@"))
                            key = key.Substring(1, key.Length - 1);
                        string type = (string)xelement.Attribute((XName)"type");
                        if (string.IsNullOrWhiteSpace(type))
                            type = string.Empty;
                        bool canBeEncrypted = false;
                        if (xelement.Attribute((XName)"encryption") != null)
                            canBeEncrypted = bool.Parse((string)xelement.Attribute((XName)"encryption"));
                        if (!canBeEncrypted && this._encryptedKeys.IndexOf(key) > 0)
                            canBeEncrypted = true;
                        string group = (string)xelement.Attribute((XName)"group");
                        if (string.IsNullOrWhiteSpace(group))
                            group = string.Empty;
                        switch (allowRequiredKeyItem)
                        {
                            case "appSettings":
                                this._requiredKeyPairs.Add(key, new XConfiguration.RequiredKeySetting(this._appSettings[key], type, canBeEncrypted, group));
                                continue;
                            default:
                                continue;
                        }
                    }
                }
            }
        }

        public class RequiredKeySetting
        {
            public string Value { get; set; }

            public string ControlType { get; set; }

            public bool CanBeEncrypted { get; set; }

            public string ControlGroupID { get; set; }

            public RequiredKeySetting(string value, string type, bool canBeEncrypted, string group)
            {
                this.Value = value;
                this.ControlType = type;
                this.CanBeEncrypted = canBeEncrypted;
                this.ControlGroupID = string.IsNullOrWhiteSpace(group) ? "zzz_Default Group" : group;
            }
        }
    }
}
