using CommunityPlugin.Objects.Enums;
using CommunityPlugin.Objects.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace CommunityPlugin.Objects.Helpers
{
    public class DbFactory
    {
        private static IDictionary<string, IDictionary<string, Type>> __typeCache = (IDictionary<string, IDictionary<string, Type>>)new Dictionary<string, IDictionary<string, Type>>(8);
        private static IDictionary<string, Assembly> __assemblyCache = (IDictionary<string, Assembly>)new Dictionary<string, Assembly>(8);

        internal DataProvider Provider { get; private set; }

        private DbFactory()
        {
        }

        internal DbFactory(string providerName)
          : this()
        {
            this.Provider = DbFactory.getProvider(providerName);
        }

        public static DbFactory Create(DataProvider provider)
        {
            return new DbFactory() { Provider = provider };
        }

        public static IDbClient CreateDBClient(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            return DbFactory.CreateDBClient(GlobalConfiguration.Connections[name]);
        }

        public static IDbClient CreateDBClient(ConnectionStringSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            return (IDbClient)new DbClient(DbFactory.Create(DbFactory.getProvider(settings.ProviderName)), GlobalConfiguration.GetConnectionString(settings.Name));
        }

        public virtual string GetTableName(string tableName)
        {
            switch (this.Provider)
            {
                case DataProvider.MSSQL:
                case DataProvider.OleDB:
                    return string.Format("[{0}]", (object)tableName);
                case DataProvider.MySQL:
                    return string.Format("`{0}`", (object)tableName);
                default:
                    return tableName;
            }
        }

        public virtual string GetColumnName(string columnName)
        {
            switch (this.Provider)
            {
                case DataProvider.MSSQL:
                case DataProvider.OleDB:
                    return string.Format("[{0}]", (object)columnName);
                case DataProvider.MySQL:
                    return string.Format("`{0}`", (object)columnName);
                default:
                    return columnName;
            }
        }

        protected virtual string GetParameterNameByColumnName(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentNullException(nameof(columnName));
            string str1 = columnName;
            char[] chArray = new char[7]
            {
        '#',
        '-',
        '\'',
        ':',
        '\\',
        '(',
        ')'
            };
            foreach (char oldChar in chArray)
                str1 = str1.Replace(oldChar, ' ').Trim();
            string str2 = str1.Replace(" ", "_");
            while (str2.IndexOf("__") >= 0)
                str2 = str2.Replace("__", "_");
            if (str2.EndsWith("_"))
                str2 = str2.Substring(0, str2.Length - 1);
            return str2;
        }

        public virtual IDataParameter CreateParameter(DataColumn column)
        {
            if (column == null)
                throw new ArgumentNullException(nameof(column));
            IDataParameter parameter = this.CreateParameter();
            parameter.ParameterName = string.Format("@{0}", (object)this.GetParameterNameByColumnName(column.ColumnName));
            parameter.DbType = !(column.DataType == typeof(DateTime)) ? (!(column.DataType == typeof(Decimal)) ? (!(column.DataType == typeof(Guid)) ? DbType.AnsiString : DbType.Guid) : DbType.Decimal) : DbType.DateTime;
            return parameter;
        }

        public virtual IDataParameter CreateParameter()
        {
            switch (this.Provider)
            {
                case DataProvider.MSSQL:
                    return (IDataParameter)new SqlParameter();
                case DataProvider.OleDB:
                    return (IDataParameter)new OleDbParameter();
                default:
                    return this.createObject("IDataParameter") as IDataParameter;
            }
        }

        public virtual IDataAdapter CreateDataAdapter()
        {
            switch (this.Provider)
            {
                case DataProvider.MSSQL:
                    return (IDataAdapter)new SqlDataAdapter();
                case DataProvider.OleDB:
                    return (IDataAdapter)new OleDbDataAdapter();
                default:
                    return this.createObject("IDataAdapter") as IDataAdapter;
            }
        }

        public virtual IDbConnection CreateConnection(string connectionString)
        {
            IDbConnection dbConnection;
            switch (this.Provider)
            {
                case DataProvider.MSSQL:
                    dbConnection = (IDbConnection)new SqlConnection();
                    break;
                case DataProvider.OleDB:
                    dbConnection = (IDbConnection)new OleDbConnection();
                    break;
                default:
                    dbConnection = this.createObject("IDbConnection") as IDbConnection;
                    break;
            }
            dbConnection.ConnectionString = connectionString;
            return dbConnection;
        }

        public IDbCommand GenerateInsertCommand(IDbConnection conn, DataTable schema)
        {
            if (conn == null)
                throw new ArgumentNullException(nameof(conn));
            if (schema == null)
                throw new ArgumentNullException(nameof(schema));
            StringBuilder stringBuilder1 = new StringBuilder();
            StringBuilder stringBuilder2 = new StringBuilder();
            IDbCommand command = conn.CreateCommand();
            command.CommandTimeout = GlobalConfiguration.CustomCommandTimeout;
            foreach (DataColumn column in (InternalDataCollectionBase)schema.Columns)
            {
                if (!column.AutoIncrement)
                {
                    if (stringBuilder1.Length > 0)
                        stringBuilder1.Append(", ");
                    stringBuilder1.AppendFormat("{0}", (object)this.GetColumnName(column.ColumnName));
                    if (stringBuilder2.Length > 0)
                        stringBuilder2.Append(", ");
                    stringBuilder2.AppendFormat("@{0}", (object)this.GetParameterNameByColumnName(column.ColumnName));
                }
            }
            command.CommandText = string.Format("Insert into {0} ({1}) Values ({2})", new object[3]
            {
        (object) this.GetTableName(schema.TableName),
        (object) stringBuilder1.ToString(),
        (object) stringBuilder2.ToString()
            });
            foreach (DataColumn column in (InternalDataCollectionBase)schema.Columns)
            {
                if (!column.AutoIncrement)
                    command.Parameters.Add((object)this.CreateParameter(column));
            }
            return command;
        }

        public IDbCommand CreateInsertCommand(IDbConnection conn, DataRow row)
        {
            if (conn == null)
                throw new ArgumentNullException(nameof(conn));
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            IDbCommand insertCommand = this.GenerateInsertCommand(conn, row.Table);
            this.FillCommandParameters(insertCommand, row);
            return insertCommand;
        }

        public IDbCommand GenerateUpdateCommand(IDbConnection conn, DataTable schema)
        {
            if (conn == null)
                throw new ArgumentNullException(nameof(conn));
            if (schema == null)
                throw new ArgumentNullException(nameof(schema));
            return this.GenerateUpdateCommand(conn, schema, schema.PrimaryKey);
        }

        public IDbCommand GenerateUpdateCommand(
          IDbConnection conn,
          DataTable schema,
          DataColumn[] keyColumns)
        {
            if (conn == null)
                throw new ArgumentNullException(nameof(conn));
            if (schema == null)
                throw new ArgumentNullException(nameof(schema));
            if (keyColumns == null)
                throw new ArgumentNullException(nameof(keyColumns));
            StringBuilder stringBuilder = new StringBuilder();
            string[] array = ((IEnumerable<DataColumn>)keyColumns).Select<DataColumn, string>((Func<DataColumn, string>)(c => c.ColumnName)).ToArray<string>();
            IDbCommand command = conn.CreateCommand();
            command.CommandTimeout = GlobalConfiguration.CustomCommandTimeout;
            foreach (DataColumn column in (InternalDataCollectionBase)schema.Columns)
            {
                if (!((IEnumerable<string>)array).Contains<string>(column.ColumnName))
                {
                    if (stringBuilder.Length > 0)
                        stringBuilder.Append(", ");
                    stringBuilder.AppendFormat("{0} = @{1}", (object)this.GetColumnName(column.ColumnName), (object)this.GetParameterNameByColumnName(column.ColumnName));
                }
            }
            for (int index = 0; index < keyColumns.Length; ++index)
            {
                stringBuilder.Append(index == 0 ? " Where " : " And ");
                stringBuilder.AppendFormat("{0} = @{1}", (object)this.GetColumnName(keyColumns[index].ColumnName), (object)this.GetParameterNameByColumnName(keyColumns[index].ColumnName));
            }
            command.CommandText = string.Format("Update {0} Set {1}", new object[2]
            {
        (object) this.GetTableName(schema.TableName),
        (object) stringBuilder.ToString()
            });
            foreach (DataColumn column in (InternalDataCollectionBase)schema.Columns)
                command.Parameters.Add((object)this.CreateParameter(column));
            return command;
        }

        public IDbCommand CreateUpdateCommand(IDbConnection conn, DataRow row)
        {
            return this.CreateUpdateCommand(conn, row, row.Table.PrimaryKey);
        }

        public IDbCommand CreateUpdateCommand(
          IDbConnection conn,
          DataRow row,
          DataColumn[] keyColumns)
        {
            if (conn == null)
                throw new ArgumentNullException(nameof(conn));
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            if (conn == null)
                throw new ArgumentNullException(nameof(conn));
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            IDbCommand updateCommand = this.GenerateUpdateCommand(conn, row.Table, keyColumns);
            this.FillCommandParameters(updateCommand, row);
            return updateCommand;
        }

        public IDbCommand GenerateDeleteCommand(IDbConnection conn, DataTable schema)
        {
            return this.GenerateDeleteCommand(conn, schema, schema.PrimaryKey);
        }

        public IDbCommand GenerateDeleteCommand(
          IDbConnection conn,
          DataTable schema,
          DataColumn[] keyColumns)
        {
            if (conn == null)
                throw new ArgumentNullException(nameof(conn));
            if (schema == null)
                throw new ArgumentNullException(nameof(schema));
            if (keyColumns == null)
                throw new ArgumentNullException(nameof(keyColumns));
            StringBuilder stringBuilder = new StringBuilder();
            ((IEnumerable<DataColumn>)keyColumns).Select<DataColumn, string>((Func<DataColumn, string>)(c => c.ColumnName)).ToArray<string>();
            IDbCommand command = conn.CreateCommand();
            command.CommandTimeout = GlobalConfiguration.CustomCommandTimeout;
            for (int index = 0; index < keyColumns.Length; ++index)
            {
                stringBuilder.Append(index == 0 ? " Where " : " And ");
                stringBuilder.AppendFormat("{0} = @{1}", (object)this.GetColumnName(keyColumns[index].ColumnName), (object)this.GetParameterNameByColumnName(keyColumns[index].ColumnName));
            }
            command.CommandText = string.Format("Delete {0} {1}", new object[2]
            {
        (object) this.GetTableName(schema.TableName),
        (object) stringBuilder.ToString()
            });
            foreach (DataColumn column in (InternalDataCollectionBase)schema.Columns)
                command.Parameters.Add((object)this.CreateParameter(column));
            return command;
        }

        public IDbCommand CreateDeleteCommand(IDbConnection conn, DataRow row)
        {
            return this.CreateDeleteCommand(conn, row, row.Table.PrimaryKey);
        }

        public IDbCommand CreateDeleteCommand(
          IDbConnection conn,
          DataRow row,
          DataColumn[] keyColumns)
        {
            if (conn == null)
                throw new ArgumentNullException(nameof(conn));
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            if (conn == null)
                throw new ArgumentNullException(nameof(conn));
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            IDbCommand deleteCommand = this.GenerateDeleteCommand(conn, row.Table, keyColumns);
            this.FillCommandParameters(deleteCommand, row, keyColumns);
            return deleteCommand;
        }

        public IDbCommand GenerateSelectKeysCommand(IDbConnection conn, DataTable schema)
        {
            return this.GenerateSelectCommand(conn, schema, schema.PrimaryKey, schema.PrimaryKey);
        }

        public IDbCommand GenerateSelectCommand(
          IDbConnection conn,
          DataTable schema,
          DataColumn[] selectedColumns,
          DataColumn[] keyColumns)
        {
            if (conn == null)
                throw new ArgumentNullException(nameof(conn));
            if (schema == null)
                throw new ArgumentNullException(nameof(schema));
            if (selectedColumns == null)
                throw new ArgumentNullException(nameof(selectedColumns));
            if (keyColumns == null)
                throw new ArgumentNullException(nameof(keyColumns));
            StringBuilder stringBuilder1 = new StringBuilder();
            StringBuilder stringBuilder2 = new StringBuilder();
            ((IEnumerable<DataColumn>)keyColumns).Select<DataColumn, string>((Func<DataColumn, string>)(c => c.ColumnName)).ToArray<string>();
            IDbCommand command = conn.CreateCommand();
            command.CommandTimeout = GlobalConfiguration.CustomCommandTimeout;
            if (selectedColumns.Length == 0)
            {
                stringBuilder1.Append("count(0)");
            }
            else
            {
                foreach (DataColumn selectedColumn in selectedColumns)
                {
                    if (stringBuilder1.Length > 0)
                        stringBuilder1.Append(", ");
                    stringBuilder1.AppendFormat("{0}", (object)this.GetColumnName(selectedColumn.ColumnName));
                }
            }
            for (int index = 0; index < keyColumns.Length; ++index)
            {
                stringBuilder2.Append(index == 0 ? " Where " : " And ");
                stringBuilder2.AppendFormat("{0} = @{1}", (object)this.GetColumnName(keyColumns[index].ColumnName), (object)this.GetParameterNameByColumnName(keyColumns[index].ColumnName));
            }
            command.CommandText = string.Format("Select {0} from {1} {2}", new object[3]
            {
        (object) stringBuilder1.ToString(),
        (object) this.GetTableName(schema.TableName),
        (object) stringBuilder2.ToString()
            });
            foreach (DataColumn keyColumn in keyColumns)
                command.Parameters.Add((object)this.CreateParameter(keyColumn));
            return command;
        }

        public void FillCommandParameters(IDbCommand cmd, DataRow row)
        {
            this.FillCommandParameters(cmd, row, row.Table.Columns.Cast<DataColumn>().ToArray<DataColumn>());
        }

        public void FillCommandParameters(IDbCommand cmd, DataRow row, DataColumn[] columns)
        {
            if (cmd == null)
                throw new ArgumentNullException(nameof(cmd));
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            foreach (DataColumn column in columns)
            {
                string index = string.Format("@{0}", (object)this.GetParameterNameByColumnName(column.ColumnName));
                (cmd.Parameters[index] as IDataParameter).Value = row[column.ColumnName];
            }
        }

        public void FillCommandParameters(IDbCommand cmd, object[] args)
        {
            if (cmd == null)
                throw new ArgumentNullException(nameof(cmd));
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (args.Length != cmd.Parameters.Count)
                throw new ArgumentException("The length of args can't match the parameters");
            for (int index = 0; index < cmd.Parameters.Count; ++index)
                (cmd.Parameters[index] as IDbDataParameter).Value = args[index];
        }

        private static DataProvider getProvider(string providerName)
        {
            if (string.IsNullOrWhiteSpace(providerName))
                throw new ArgumentNullException(nameof(providerName));
            switch (providerName.ToLower())
            {
                case "system.data.sqlclient":
                case "system.data.mssql":
                    return DataProvider.MSSQL;
                case "system.data.mysql":
                    return DataProvider.MySQL;
                case "system.data.sqlite":
                    return DataProvider.SQLite;
                case "system.data.olddb":
                    return DataProvider.OleDB;
                default:
                    try
                    {
                        return (DataProvider)Enum.Parse(typeof(DataProvider), providerName, true);
                    }
                    catch (Exception ex)
                    {
                        return DataProvider.Unknown;
                    }
            }
        }

        protected object createObject(string className)
        {
            Type dbType = DbFactory.getDbType(this.Provider, className);
            if (dbType == (Type)null)
                throw new ArgumentNullException(string.Format("Invalid classname:{0}", (object)className));
            ConstructorInfo constructor = dbType.GetConstructor(new Type[0]);
            if (constructor == (ConstructorInfo)null)
                throw new Exception(string.Format("load type:{0} failed.", (object)className));
            return constructor.Invoke(new object[0]);
        }

        private static Type getDbType(DataProvider provider, string objectName)
        {
            string providerName = provider.ToString().ToLower();
            if (!DbFactory.__typeCache.ContainsKey(providerName))
            {
                lock (DbFactory.__typeCache)
                {
                    if (!DbFactory.__typeCache.ContainsKey(providerName))
                        DbFactory.__typeCache.Add(providerName, (IDictionary<string, Type>)new Dictionary<string, Type>(8));
                }
            }
            IDictionary<string, Type> dictionary = DbFactory.__typeCache[providerName];
            if (dictionary.ContainsKey(objectName))
                return dictionary[objectName];
            typeof(DbFactory).Assembly.GetManifestResourceNames();
            Type type = (Type)null;
            Stream definedStream = DbFactory.getDefinedStream();
            try
            {
                if (definedStream == null)
                    throw new Exception(string.Format("Can't find object:{0} for provider:{1}.", (object)provider, (object)objectName));
                XDocument xdocument = XDocument.Load(definedStream);
                string name = xdocument.Document.Descendants((XName)"Item").Where<XElement>((Func<XElement, bool>)(e => (string)e.Parent.Attribute((XName)"name") == providerName && (string)e.Attribute((XName)"name") == objectName)).Select<XElement, string>((Func<XElement, string>)(e => (string)e)).SingleOrDefault<string>();
                string assemblyName = xdocument.Document.Descendants((XName)"Provider").Where<XElement>((Func<XElement, bool>)(e => (string)e.Attribute((XName)"name") == providerName)).Select<XElement, string>((Func<XElement, string>)(e => (string)e.Attribute((XName)"assembplyName"))).SingleOrDefault<string>();
                if (!string.IsNullOrWhiteSpace(assemblyName))
                    type = DbFactory.getAssembly(assemblyName).GetType(name);
            }
            finally
            {
                definedStream?.Close();
            }
            if (type == (Type)null)
                throw new ArgumentNullException("");
            lock (dictionary)
            {
                if (!dictionary.ContainsKey(objectName))
                    dictionary.Add(objectName, type);
            }
            return type;
        }

        private static Assembly getAssembly(string assemblyName)
        {
            if (string.IsNullOrWhiteSpace(assemblyName))
                throw new ArgumentNullException(nameof(assemblyName));
            if (!DbFactory.__assemblyCache.ContainsKey(assemblyName))
            {
                lock (DbFactory.__assemblyCache)
                {
                    if (!DbFactory.__assemblyCache.ContainsKey(assemblyName))
                    {
                        Assembly assembly = (Assembly)null;
                        try
                        {
                            try
                            {
                                assembly = Assembly.Load(assemblyName);
                            }
                            catch (Exception ex)
                            {
                            }
                            if (assemblyName == null)
                                assembly = Assembly.LoadFile(string.Format("{0}//{1}.dll", (object)Environment.CurrentDirectory, (object)assemblyName));
                            if (assemblyName == null)
                                throw new NullReferenceException(string.Format("Can't find assembly:{0} in application directory or global cache", (object)assemblyName));
                            DbFactory.__assemblyCache.Add(assemblyName, assembly);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(string.Format("Can't load assembly:{0}", (object)assemblyName), ex);
                        }
                    }
                }
            }
            return DbFactory.__assemblyCache[assemblyName];
        }

        private static Stream getDefinedStream()
        {
            string path = string.Format("{0}{1}.dll", (object)Environment.CurrentDirectory, (object)"Provider.xml");
            if (File.Exists(path))
                return (Stream)File.OpenRead(path);
            foreach (string manifestResourceName in typeof(DbFactory).Assembly.GetManifestResourceNames())
            {
                if (manifestResourceName.EndsWith("Provider.xml"))
                    return typeof(DbFactory).Assembly.GetManifestResourceStream(manifestResourceName);
            }
            return (Stream)null;
        }
    }
}
