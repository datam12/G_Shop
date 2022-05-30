using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;

namespace DatabaseHelper
{
    public class Database<TConnection, TCommand, TTransaction, TParameter, TDataReader, TAdapter, TBuilder> : IDisposable
        where TConnection : DbConnection, new()
        where TCommand : DbCommand, new()
        where TTransaction : DbTransaction
        where TParameter : DbParameter, new()
        where TDataReader : DbDataReader
        where TAdapter : DbDataAdapter, new()
        where TBuilder : DbCommandBuilder, new()
    {
        private TConnection _connection;
        private TTransaction _transaction;
        private readonly bool _useSingletonConncetion;

        public string ConnectionString { get; protected set; }

        public Database(string connectionString, bool useSingletonConncetion = true)
        {
            ConnectionString = connectionString;
            _useSingletonConncetion = useSingletonConncetion;
        }

        public virtual TConnection GetConnection()
        {
            // Singleton pattern
            if (_connection == default || !_useSingletonConncetion)
            {
                _connection = new TConnection
                {
                    ConnectionString = ConnectionString
                };
            }
            return _connection;
        }

        public virtual void OpenConnection()
        {
            if (!_useSingletonConncetion)
                throw new Exception("Open connection is supported only in single connection mode");
            this.GetConnection();
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
        }

        public virtual void CloseConnection()
        {
            if (!_useSingletonConncetion)
                throw new Exception("Close connection is supported only in single connection mode");
            _connection.Close();
        }

        public virtual TCommand GetCommand(string commandText, CommandType commandType, params TParameter[] parameters)
        {
            var command = GetConnection().CreateCommand();
            command.CommandText = commandText;
            command.CommandType = commandType;
            foreach (var p in parameters)
            {
                command.Parameters.Add(p);
            }
            if (_transaction != default)
                command.Transaction = _transaction;
            return (TCommand)command;
        }

        public virtual TCommand GetCommand(string commandText, params TParameter[] parameters)
        {
            return GetCommand(commandText, CommandType.Text, parameters);
        }

        public virtual object ExecuteScalar(string commandText, CommandType commandType, params TParameter[] parameters)
        {
            var command = GetCommand(commandText, commandType, parameters);
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                    command.Connection.Open();
                return command.ExecuteScalar();
            }
            finally
            {
                if (!_useSingletonConncetion)
                    command.Connection.Close();
            }
        }

        public virtual object ExecuteScalar(string commandText, params TParameter[] parameters)
        {
            return ExecuteScalar(commandText, CommandType.Text, parameters);
        }

        public virtual int ExecuteNonQuerry(string commandText, CommandType commandType, params TParameter[] parameters)
        {
            var command = GetCommand(commandText, commandType, parameters);
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                    command.Connection.Open();
                return command.ExecuteNonQuery();
            }
            finally
            {
                if (!_useSingletonConncetion)
                    command.Connection.Close();
            }
        }

        public virtual int ExecuteNonQuerry(string commandText, params TParameter[] parameters)
        {
            return ExecuteNonQuerry(commandText, CommandType.Text, parameters);
        }

        public virtual TDataReader ExecuteReader(string commandText, CommandType commandType, params TParameter[] parameters)
        {
            var command = GetCommand(commandText, commandType, parameters);
            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();
            if (!_useSingletonConncetion)
                return (TDataReader)command.ExecuteReader(CommandBehavior.CloseConnection);
            return (TDataReader)command.ExecuteReader();
        }

        public virtual TDataReader ExecuteReader(string commandText, params TParameter[] parameters)
        {
            return ExecuteReader(commandText, CommandType.Text, parameters);
        }

        public virtual DataTable GetTable(string commandText, CommandType commandType, params TParameter[] parameters)
        {
            var command = GetCommand(commandText, commandType, parameters);
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                    command.Connection.Open();
                var dataTable = new DataTable();
                dataTable.Load(command.ExecuteReader());
                return dataTable;
            }
            finally
            {
                if (!_useSingletonConncetion)
                    command.Connection.Close();
            }
        }

        public virtual DataTable GetTable(string commandText, params TParameter[] parameters)
        {
            return GetTable(commandText, CommandType.Text, parameters);
        }

        public virtual void UpdateTable(DataTable table)
        {
            if (table.TableName == "") throw new MissingFieldException("Table has no name");
            string selectSql = $"select * from {table.TableName}";

            var adapter = new TAdapter();
            adapter.SelectCommand = GetCommand(selectSql);
            var commandBuilder = new TBuilder();
            commandBuilder.DataAdapter = adapter;
            adapter.InsertCommand = commandBuilder.GetInsertCommand(true);
            adapter.UpdateCommand = commandBuilder.GetUpdateCommand(true);
            adapter.DeleteCommand = commandBuilder.GetDeleteCommand(true);
            
            adapter.Update(table);
        }

        public virtual void BeginTransaction()
        {
            if (!_useSingletonConncetion)
                throw new Exception("Transaction is supported only in single connection mode");
            if (_transaction != default)
                throw new Exception("There is active transaction");
            var connection = GetConnection();
            connection.Open();
            _transaction = (TTransaction)connection.BeginTransaction();
        }

        public virtual void CommitTransaction()
        {
            if (!_useSingletonConncetion)
                throw new Exception("Transaction is supported only in single connection mode");
            if (_transaction == default)
                throw new Exception("There is no active transaction");
            _transaction.Commit();
            _transaction = default;
        }

        public virtual void RollbackTransaction()
        {
            if (!_useSingletonConncetion)
                throw new Exception("Transaction is supported only in single connection mode");
            if (_transaction == default)
                throw new Exception("There is no active transaction");
            _transaction.Rollback();
            _transaction = default;
        }

        public virtual void ExecuteTransaction(params TCommand[] commands)
        {
            if (commands.Length == 0)
                throw new ArgumentException("Command object was not supplied");

            this.BeginTransaction();
            try
            {
                foreach (var command in commands)
                {
                    command.Transaction = _transaction;
                    command.ExecuteNonQuery();
                }
                this.CommitTransaction();
            }
            catch
            {
                this.RollbackTransaction();
                throw;
            }
        }

        public virtual void Dispose()
        {
            _connection?.Close();
        }

        ~Database()
        {
            this.Dispose();
        }
    }
}
