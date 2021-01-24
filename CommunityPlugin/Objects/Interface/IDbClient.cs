using CommunityPlugin.Objects.Helpers;
using System;
using System.ComponentModel;
using System.Data;

namespace CommunityPlugin.Objects.Interface
{
    public interface IDbClient : IDisposable
    {
        event CancelEventHandler Opening;

        event EventHandler Opened;

        event CancelEventHandler Closing;

        event EventHandler Closed;

        string ConnectionString { get; set; }

        DbFactory Factory { get; }

        IDbConnection Connection { get; }

        void Open();

        void Close();

        IDbTransaction BeginTransaction();

        IDbTransaction BeginTransaction(IsolationLevel il);

        void Commit();

        void Rollback();

        IDataAdapter CreateAdapter();

        IDbConnection CreateConnection();

        IDbConnection CreateConnection(string connectionString);

        IDbCommand CreateCommand();

        IDbCommand CreateCommand(string text);

        IDbCommand CloneCommand(IDbCommand cmd);

        int GetIdentity();

        int ExecuteNonQuery(IDbCommand cmd);

        int ExecuteNonQuery(string text);

        object ExecuteScalar(IDbCommand cmd);

        object ExecuteScalar(string text);

        IDataReader ExecuteReader(IDbCommand cmd);

        IDataReader ExecuteReader(string text);

        IDataReader ExecuteReader(IDbCommand cmd, CommandBehavior behavior);

        IDataReader ExecuteReader(string text, CommandBehavior behavior);

        int Fill(string comamndText, DataSet dataSet);

        int Fill(string comamndText, DataTable dataTable);

        int Fill(string comamndText, DataSet dataSet, string srcTable);

        int Fill(
          string comamndText,
          DataSet dataSet,
          int startRecord,
          int maxRecords,
          string srcTable);

        int InsertTable(DataTable table);

        int InsertRow(DataRow row);

        int UpdateTable(DataTable table);

        int UpdateTable(DataTable table, DataColumn[] keyColumns);

        int UpdateRow(DataRow row);

        int UpdateRow(DataRow row, DataColumn[] uniqueColumns);

        int DeleteTable(DataTable schema, object[] args);

        int DeleteRow(DataRow row);

        int DeleteRow(DataRow row, DataColumn[] uniqueColumns);

        bool ExistRecords(DataTable schema, object[] args);

        int GetRecords(DataTable schema, object[] args);
    }
}
