using CommunityPlugin.Objects.BaseClasses;
using CommunityPlugin.Objects.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;

namespace CommunityPlugin.Objects.Helpers
{
    public class DbClient : IDbClient, IDisposable
    {
        private List<string> truncatedColumns = new List<string>();
        public List<string> wrongDataTypeColumns = new List<string>();
        private const string ClassName = "Utility4.Data.DbClient";
        public Func<string, string, string> GetSize;
        public Func<string, string, Enums.ValueType> GetColumnType;

        public event CancelEventHandler Opening;

        public event EventHandler Opened;

        public event CancelEventHandler Closing;

        public event EventHandler Closed;

        public DbFactory Factory { get; set; }

        public string ConnectionString
        {
            get
            {
                return this.Connection.ConnectionString;
            }
            set
            {
                if (this.Connection == null)
                    this.Connection = this.Factory.CreateConnection(value);
                else
                    this.Connection.ConnectionString = value;
            }
        }

        protected virtual string ProviderName
        {
            get
            {
                return string.Empty;
            }
        }

        public IDbConnection Connection { get; set; }

        public IDbTransaction Transaction { get; set; }

        public IList<IDbCommand> Commands { get; set; }

        protected internal DbClient(DbFactory factory, string name)
          : this()
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            this.Factory = factory;
            string str = string.Empty;
            try
            {
                str = GlobalConfiguration.Connections[name].ConnectionString;
            }
            catch (Exception ex)
            {
            }
            this.ConnectionString = string.IsNullOrWhiteSpace(str) ? name : str;
        }

        protected DbClient()
        {
            this.Connection = (IDbConnection)null;
            this.Transaction = (IDbTransaction)null;
            this.Commands = (IList<IDbCommand>)new List<IDbCommand>(8);
        }

        public void Open()
        {
            CancelEventArgs e = new CancelEventArgs(false);
            if (this.Opening != null)
            {
                this.Opening((object)this, e);
                if (e.Cancel)
                    return;
            }
            this.Connection.Open();
            if (this.Opened == null)
                return;
            this.Opened((object)this, EventArgs.Empty);
        }

        protected internal void OpenOrReady()
        {
            try
            {
                if (this.Connection.State != ConnectionState.Closed && this.Connection.State != ConnectionState.Broken)
                    return;
                //GlobalTracer.TraceVerboseFormat("The connecion (state:{0}) was broken or closed, now try to reopen.", (object)this.Connection.State);
                this.Open();
            }
            catch (Exception ex)
            {
                //GlobalTracer.TraceErrorFormat("Unknown issue occur when try to open database connection, details:{0}", (object)ex.Message);
            }
        }

        public void Close()
        {
            if (this.Connection == null || this.Connection.State == ConnectionState.Closed)
                return;
            CancelEventArgs e = new CancelEventArgs(false);
            if (this.Closing != null)
            {
                this.Closing((object)this, e);
                if (e.Cancel)
                    return;
            }
            this.Connection.Close();
            if (this.Closed == null)
                return;
            this.Closed((object)this, EventArgs.Empty);
        }

        public IDbTransaction BeginTransaction()
        {
            try
            {
                return this.BeginTransaction(IsolationLevel.ReadCommitted);
            }
            catch (Exception ex)
            {
                return (IDbTransaction)null;
            }
        }

        public virtual IDbTransaction BeginTransaction(IsolationLevel il)
        {
            if (this.Transaction != null)
                throw new InvalidOperationException("transaction was already exist.");
            this.OpenOrReady();
            this.Transaction = this.Connection.BeginTransaction(il);
            return this.Transaction;
        }

        public void Commit()
        {
            if (this.Transaction == null)
                throw new InvalidOperationException("transaction wasn't created.");
            try
            {
                this.Transaction.Commit();
            }
            finally
            {
                this.Transaction.Dispose();
                this.Transaction = (IDbTransaction)null;
            }
        }

        public void Rollback()
        {
            if (this.Transaction == null)
                throw new InvalidOperationException("transaction wasn't created.");
            try
            {
                this.Transaction.Rollback();
            }
            finally
            {
                this.Transaction.Dispose();
                this.Transaction = (IDbTransaction)null;
            }
        }

        public int InsertTable(DataTable table)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));
            if (table.Rows.Count == 0)
                return 0;
            int num = 0;
            IDbCommand insertCommand = this.Factory.GenerateInsertCommand(this.Connection, table);
            foreach (DataRow row in (InternalDataCollectionBase)table.Rows)
            {
                try
                {
                    using (IDbCommand cmd = this.CloneCommand(insertCommand))
                    {
                        this.Factory.FillCommandParameters(cmd, row);
                        this.OutputCommandInfo(cmd);
                        this.OpenOrReady();
                        num += cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    //GlobalTracer.TraceError(string.Format("Insert row failed, details:{0}", (object)ex.Message), string.Format("{0}.InsertTable", (object)"Utility4.Data.DbClient"));
                }
            }
            return num;
        }

        public int InsertRow(DataRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            using (IDbCommand cmd = this.CloneCommand(this.Factory.CreateInsertCommand(this.Connection, row)))
            {
                this.OutputCommandInfo(cmd);
                this.OpenOrReady();
                return cmd.ExecuteNonQuery();
            }
        }

        public int UpdateTable(DataTable table)
        {
            return this.UpdateTable(table, table.PrimaryKey);
        }

        public int UpdateTable(DataTable table, DataColumn[] keyColumns)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));
            int num = 0;
            IDbCommand updateCommand = this.Factory.GenerateUpdateCommand(this.Connection, table);
            foreach (DataRow row in (InternalDataCollectionBase)table.Rows)
            {
                try
                {
                    using (IDbCommand cmd = this.CloneCommand(updateCommand))
                    {
                        this.Factory.FillCommandParameters(cmd, row);
                        this.OutputCommandInfo(cmd);
                        this.OpenOrReady();
                        num += cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    //GlobalTracer.TraceErrorFormat(string.Format("Update row failed, details:{0}", (object)ex.Message), (object)string.Format("{0}.UpdateTable", (object)"Utility4.Data.DbClient"));
                }
            }
            return num;
        }

        public int UpdateRow(DataRow row)
        {
            return this.UpdateRow(row, row.Table.PrimaryKey);
        }

        public int UpdateRow(DataRow row, DataColumn[] keyColumns)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            if (keyColumns == null)
                throw new ArgumentNullException(nameof(keyColumns));
            using (IDbCommand cmd = this.CloneCommand(this.Factory.CreateUpdateCommand(this.Connection, row, keyColumns)))
            {
                this.OutputCommandInfo(cmd);
                this.OpenOrReady();
                return cmd.ExecuteNonQuery();
            }
        }

        public int DeleteTable(DataTable schema, object[] args)
        {
            if (schema == null)
                throw new ArgumentNullException(nameof(schema));
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            using (IDbCommand deleteCommand = this.Factory.GenerateDeleteCommand(this.Connection, schema))
            {
                this.Factory.FillCommandParameters(deleteCommand, args);
                this.OutputCommandInfo(deleteCommand);
                this.OpenOrReady();
                return deleteCommand.ExecuteNonQuery();
            }
        }

        public int DeleteRow(DataRow row)
        {
            return this.DeleteRow(row, row.Table.PrimaryKey);
        }

        public int DeleteRow(DataRow row, DataColumn[] keyColumns)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            if (keyColumns == null)
                throw new ArgumentNullException(nameof(keyColumns));
            using (IDbCommand cmd = this.CloneCommand(this.Factory.CreateDeleteCommand(this.Connection, row, keyColumns)))
            {
                this.OutputCommandInfo(cmd);
                this.OpenOrReady();
                return cmd.ExecuteNonQuery();
            }
        }

        public List<string> TruncatedColumns
        {
            get
            {
                return this.truncatedColumns;
            }
        }

        public List<string> WrongDataTypeColumns
        {
            get
            {
                return this.wrongDataTypeColumns;
            }
        }

        protected internal virtual void OutputCommandInfo(IDbCommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException(nameof(cmd));
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("Output command infomation\r\n#output start\r\nText:\r\n{0}\r\n", (object)cmd.CommandText);
            string[] array = cmd.CommandText.Split(new string[1]
            {
        " "
            }, StringSplitOptions.RemoveEmptyEntries);
            int num = Math.Max(Array.FindIndex<string>(array, (Predicate<string>)(cp => cp.ToLower() == "into")), Array.FindIndex<string>(array, (Predicate<string>)(cp => cp.ToLower() == "update")));
            string s1 = string.Empty;
            string str1 = string.Empty;
            if (num >= 0)
            {
                int index = num + 1;
                str1 = array[index].Replace("[", string.Empty).Replace("]", string.Empty);
            }
            if (cmd.Parameters.Count > 0)
            {
                stringBuilder.Append("Parameters:\r\n");
                foreach (IDataParameter parameter in (IEnumerable)cmd.Parameters)
                {
                    if (this.GetSize != null)
                        s1 = this.GetSize(str1, parameter.ParameterName);
                    int result1 = 0;
                    if (int.TryParse(s1, out result1) && parameter.Value.ToString().Length > result1)
                    {
                        string str2 = parameter.ParameterName.Replace("@", string.Empty);
                        this.truncatedColumns.Add(string.Format("The length of the data of {0}.{1} is {2} which exceeds the allowed size of {3}.", (object)str1, (object)str2, (object)parameter.Value.ToString().Length, (object)result1));
                    }
                    string str3 = !string.IsNullOrEmpty(s1) ? string.Format("(DB: {0}; Data: {1})", (object)s1, (object)parameter.Value.ToString().Length) : string.Empty;
                    stringBuilder.AppendFormat("{0}={1}\t{2}{3}\r\n", (object)parameter.ParameterName, parameter.Value, (object)parameter.DbType, (object)str3);
                    string s2 = parameter.Value == null ? string.Empty : parameter.Value.ToString().Trim();
                    Enums.ValueType valueType1 = this.GetColumnType(str1, parameter.ParameterName);
                    if (s2 != string.Empty && valueType1 != Enums.ValueType.Unknown)
                    {
                        double result2 = 0.0;
                        DateTime result3 = DateTime.MinValue;
                        string empty = string.Empty;
                        Enums.ValueType valueType2 = !double.TryParse(s2, out result2) ? (!DateTime.TryParse(s2, out result3) ? Enums.ValueType.String : Enums.ValueType.DateTime) : Enums.ValueType.Numeric;
                        string str2 = parameter.ParameterName.Replace("@", string.Empty);
                        if (valueType1 != valueType2 && valueType1 != Enums.ValueType.String)
                            this.wrongDataTypeColumns.Add(string.Format("The value {0} is type {1} which does not match the database type of {2} for {3}.{4}.", (object)s2, (object)valueType2.ToString(), (object)valueType1.ToString(), (object)str1, (object)str2));
                        stringBuilder.AppendFormat("{0}.{1} is {2}. Data is {3}", (object)str1, (object)str2, (object)valueType1.ToString(), (object)valueType2.ToString());
                    }
                }
            }
            stringBuilder.AppendLine("#output end");
            //GlobalTracer.TraceVerbose(stringBuilder.ToString(), string.Format("{0}.OutputCommandInfo", (object)"Utility4.Data.DbClient"));
        }

        public bool ExistRecords(DataTable schema, object[] args)
        {
            return this.GetRecords(schema, args) > 0;
        }

        public int GetRecords(DataTable schema, object[] args)
        {
            if (schema == null)
                throw new ArgumentNullException(nameof(schema));
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            using (IDbCommand cmd = this.CloneCommand(this.Factory.GenerateSelectCommand(this.Connection, schema, new DataColumn[0], schema.PrimaryKey)))
            {
                this.Factory.FillCommandParameters(cmd, args);
                this.OutputCommandInfo(cmd);
                this.OpenOrReady();
                string s = string.Format("{0}", cmd.ExecuteScalar());
                int result = 0;
                if (!int.TryParse(s, out result))
                    result = 0;
                //GlobalTracer.TraceVerboseFormat("Get records:{0}", (object)result);
                return result;
            }
        }

        public int Fill(string commandText, DataSet dataSet)
        {
            return this.Fill(commandText, (Func<DbDataAdapter, int>)(adapter => adapter.Fill(dataSet)));
        }

        public int Fill(string commandText, DataTable dataTable)
        {
            return this.Fill(commandText, (Func<DbDataAdapter, int>)(adapter => adapter.Fill(dataTable)));
        }

        public int Fill(string commandText, DataSet dataSet, string srcTable)
        {
            return this.Fill(commandText, (Func<DbDataAdapter, int>)(adapter => adapter.Fill(dataSet, srcTable)));
        }

        public int Fill(
          string commandText,
          DataSet dataSet,
          int startRecord,
          int maxRecords,
          string srcTable)
        {
            return this.Fill(commandText, (Func<DbDataAdapter, int>)(adapter => adapter.Fill(dataSet, startRecord, maxRecords, srcTable)));
        }

        protected virtual int Fill(string commandText, Func<DbDataAdapter, int> fill)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentNullException("comamndText");
            this.OpenOrReady();
            using (DbDataAdapter adapter = this.CreateAdapter() as DbDataAdapter)
            {
                using (IDbCommand command = this.CreateCommand())
                {
                    command.CommandText = commandText;
                    command.CommandType = CommandType.Text;
                    adapter.SelectCommand = command as DbCommand;
                    this.OutputCommandInfo(command);
                    return fill(adapter);
                }
            }
        }

        public virtual int GetIdentity()
        {
            return 0;
        }

        public IDbConnection CreateConnection()
        {
            return this.CreateConnection(string.Empty);
        }

        public IDbConnection CreateConnection(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));
            IDbConnection connection = this.Factory.CreateConnection(connectionString);
            if (connectionString != string.Empty)
                connection.ConnectionString = connectionString;
            return connection;
        }

        public IDbCommand CreateCommand()
        {
            return this.CreateCommand(string.Empty);
        }

        public IDbCommand CreateCommand(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            IDbCommand command = this.Connection.CreateCommand();
            command.CommandTimeout = GlobalConfiguration.CustomCommandTimeout;
            if (text != string.Empty)
                command.CommandText = text;
            if (this.Transaction != null)
                command.Transaction = this.Transaction;
            if (!this.Commands.Contains(command))
                this.Commands.Add(command);
            return command;
        }

        public virtual IDataAdapter CreateAdapter()
        {
            return this.Factory.CreateDataAdapter();
        }

        public virtual IDbCommand CloneCommand(IDbCommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException(nameof(cmd));
            IDbCommand command = this.CreateCommand();
            command.CommandText = cmd.CommandText;
            command.CommandType = cmd.CommandType;
            command.CommandTimeout = cmd.CommandTimeout;
            for (int index = 0; index < cmd.Parameters.Count; ++index)
            {
                IDbDataParameter parameter1 = cmd.Parameters[index] as IDbDataParameter;
                if (parameter1 is ICloneable)
                {
                    command.Parameters.Add((parameter1 as ICloneable).Clone());
                }
                else
                {
                    IDbDataParameter parameter2 = command.CreateParameter();
                    parameter2.ParameterName = parameter1.ParameterName;
                    parameter2.Direction = parameter1.Direction;
                    parameter2.Value = parameter1.Value;
                    parameter2.SourceColumn = parameter1.SourceColumn;
                    parameter2.DbType = parameter1.DbType;
                    parameter2.SourceVersion = parameter1.SourceVersion;
                    command.Parameters.Add((object)parameter2);
                }
            }
            if (cmd.Transaction != null)
                command.Transaction = cmd.Transaction;
            else if (this.Transaction != null)
                command.Transaction = this.Transaction;
            return command;
        }

        public int ExecuteNonQuery(IDbCommand cmd)
        {
            this.OpenOrReady();
            return cmd.ExecuteNonQuery();
        }

        public int ExecuteNonQuery(string text)
        {
            using (IDbCommand command = this.CreateCommand(text))
            {
                this.OpenOrReady();
                return command.ExecuteNonQuery();
            }
        }

        public object ExecuteScalar(IDbCommand cmd)
        {
            this.OpenOrReady();
            return cmd.ExecuteScalar();
        }

        public object ExecuteScalar(string text)
        {
            using (IDbCommand command = this.CreateCommand(text))
            {
                this.OpenOrReady();
                return command.ExecuteScalar();
            }
        }

        public IDataReader ExecuteReader(IDbCommand cmd)
        {
            this.OpenOrReady();
            return cmd.ExecuteReader();
        }

        public IDataReader ExecuteReader(string text)
        {
            using (IDbCommand command = this.CreateCommand(text))
            {
                this.OpenOrReady();
                return command.ExecuteReader();
            }
        }

        public IDataReader ExecuteReader(IDbCommand cmd, CommandBehavior behavior)
        {
            this.OpenOrReady();
            return cmd.ExecuteReader(behavior);
        }

        public IDataReader ExecuteReader(string text, CommandBehavior behavior)
        {
            using (IDbCommand command = this.CreateCommand(text))
            {
                this.OpenOrReady();
                return command.ExecuteReader(behavior);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            if (this.Commands != null)
            {
                for (int index = 0; index < this.Commands.Count; ++index)
                {
                    try
                    {
                        this.Commands[index].Dispose();
                    }
                    catch (Exception ex)
                    {
                    }
                }
                this.Commands.Clear();
                this.Commands = (IList<IDbCommand>)null;
            }
            if (this.Transaction != null)
            {
                this.Transaction.Dispose();
                this.Transaction = (IDbTransaction)null;
            }
            if (this.Connection == null)
                return;
            this.Close();
            if (this.Connection == null)
                return;
            try
            {
                this.Connection.Dispose();
                this.Connection = (IDbConnection)null;
            }
            catch (Exception ex)
            {
            }
        }

        public static IDbClient CreateClient(string providerName)
        {
            return (IDbClient)null;
        }
    }
}
