using System.Data.SqlClient;
using System.Data.SQLite;
using System.Data.Common;
using System.Data;
using System.Data.Sql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tuvalu;
using static Tuvalu.Program;
namespace Tuvalu.DB
{
    public struct Data
    {
        public DBconnector DB;
        public List<TTasks.TTask> Tasklist;
        public string json_config;

        public Data(DBconnector db, List<TTasks.TTask> tasklist)
        {
            DB = db;
            Tasklist = tasklist;
            json_config = null;
        }
    }

    public class DBconnector
    {
        private string _connectionString;
        private string _provider;
        private DbConnection? _connection;
        private DbCommand? _command;
        private DbDataReader? _reader;
        private DbDataAdapter? _adapter;
        private DataSet? _dataSet;
        private DataTable _dataTable;
        private string _sql;
        private string _tableName;
        private string _dbName;
        private string _dbPath;
        private string _dbType;
        private string _dbProvider;
        private string _dbConnectionString;
        private string _dbCommand;
        private string _dbTable;
        private string _dbQuery;
        private string _dbQueryType;
        private string _dbQueryValue;
        private string _dbQueryColumn;
        private string _dbQueryCondition;
        private string _dbQueryConditionValue;
        private string _dbQueryConditionColumn;
        private string _dbQueryConditionOperator;
        private string _dbQueryConditionType;

        public DBconnector()
        {
            _connectionString = "";
            _provider = "";
            _connection = null;
            _command = null;
            _reader = null;
            _adapter = null;
            _dataSet = null;
            _dataTable = new DataTable();
            _sql = "";
            _tableName = "";
            _dbName = "";
            _dbPath = "";
            _dbType = "";
            _dbProvider = "";
            _dbConnectionString = "";
            _dbCommand = "";
            _dbTable = "";
            _dbQuery = "";
            _dbQueryType = "";
            _dbQueryValue = "";
            _dbQueryColumn = "";
            _dbQueryCondition = "";
            _dbQueryConditionValue = "";
            _dbQueryConditionColumn = "";
            _dbQueryConditionOperator = "";
            _dbQueryConditionType = "";
        }

        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        public string Provider
        {
            get { return _provider; }
            set { _provider = value; }
        }

        public DbConnection? Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        public DbCommand? Command
        {
            get { return _command; }
            set { _command = value; }
        }

        public DbDataReader? Reader
        {
            get { return _reader; }
            set { _reader = value; }
        }

        public DbDataAdapter? Adapter
        {
            get { return _adapter; }
            set { _adapter = value; }
        }

        public DataSet? DataSet
        {
            get { return _dataSet; }
            set { _dataSet = value; }
        }

        public DataTable DataTable
        {
            get { return _dataTable; }
            set { _dataTable = value; }
        }

        public string SQL
        {
            get { return _sql; }
            set { _sql = value; }
        }

        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        public string DBName
        {
            get { return _dbName; }
            set { _dbName = value; }
        }

        public string DBPath
        {
            get { return _dbPath; }
            set { _dbPath = value; }
        }

        public string DBType
        {
            get { return _dbType; }
            set { _dbType = value; }
        }

        public string DBProvider
        {
            get { return _dbProvider; }
            set { _dbProvider = value; }
        }

        public string DBConnectionString
        {
            get { return _dbConnectionString; }
            set { _dbConnectionString = value; }
        }

        public string DBCommand
        {
            get { return _dbCommand; }
            set { _dbCommand = value; }
        }

        public string DBTable
        {
            get { return _dbTable; }
            set { _dbTable = value; }
        }

        public string DBQuery
        {
            get { return _dbQuery; }
            set { _dbQuery = value; }
        }

        public string DBQueryType
        {
            get { return _dbQueryType; }
            set { _dbQueryType = value; }
        }

        public string DBQueryValue
        {
            get { return _dbQueryValue; }
            set { _dbQueryValue = value; }
        }

        public string DBQueryColumn
        {
            get { return _dbQueryColumn; }
            set { _dbQueryColumn = value; }
        }

        public string DBQueryCondition
        {
            get { return _dbQueryCondition; }
            set { _dbQueryCondition = value; }
        }

        public string DBQueryConditionValue
        {
            get { return _dbQueryConditionValue; }
            set { _dbQueryConditionValue = value; }
        }

        public string DBQueryConditionColumn
        {
            get { return _dbQueryConditionColumn; }
            set { _dbQueryConditionColumn = value; }
        }

        public string DBQueryConditionOperator
        {
            get { return _dbQueryConditionOperator; }
            set { _dbQueryConditionOperator = value; }
        }

        public string DBQueryConditionType
        {
            get { return _dbQueryConditionType; }
            set { _dbQueryConditionType = value; }
        }

        public static void CreateDB(DBconnector db)
        {
            if (db.DBType == "SQLite")
            {
                if (string.IsNullOrEmpty(db.DBPath))
                {
                    db.DBPath = Globals.AppDBFile;
                }
                SQLiteConnection.CreateFile(db.DBPath);
            }
            else
            {
                throw new NotSupportedException($"Database type {db.DBType} not supported");
            }
        }

        public static bool DBExists(DBconnector db)
        {
            bool result = false;
            if (db.DBType == "SQLite")
            {
                if (string.IsNullOrEmpty(db.DBPath))
                {
                    db.DBPath = Globals.AppDBFile;
                }
                result = System.IO.File.Exists(db.DBPath);
            }
            else
            {
                throw new Exception("Database type not supported");
            }
            return result;
        }

        public static int ExecuteNonQuery(DBconnector db)
        {
            int result = 0;
            if (db.DBType == "SQLite")
            {
                if (string.IsNullOrEmpty(db.DBConnectionString))
                {
                    throw new Exception("Database connection string cannot be empty");
                }
                using (SQLiteConnection connection = new SQLiteConnection(db.DBConnectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = db.DBCommand;
                        result = command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            else
            {
                throw new Exception("Database type not supported");
            }
            return result;
        }

        public static async Task<int> ExecuteNonQueryAsync(DBconnector db)
        {
            if (db.DBType != "SQLite")
                throw new NotSupportedException($"Database type {db.DBType} not supported");

            if (string.IsNullOrEmpty(db.DBConnectionString))
                throw new Exception("Database connection string cannot be empty");

            using var connection = new SQLiteConnection(db.DBConnectionString);
            await connection.OpenAsync();
            using var command = new SQLiteCommand(db.DBCommand, connection);
            return await command.ExecuteNonQueryAsync();
        }

        public static int ExecuteScalar(DBconnector db)
        {
            int result = 0;
            if (db.DBType == "SQLite")
            {
                if (string.IsNullOrEmpty(db.DBConnectionString))
                {
                    throw new Exception("Database connection string cannot be empty");
                }
                using (SQLiteConnection connection = new SQLiteConnection(db.DBConnectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = db.DBCommand;
                        result = Convert.ToInt32(command.ExecuteScalar());
                    }
                    connection.Close();
                }
            }
            else
            {
                throw new Exception("Database type not supported");
            }
            return result;
        }

        public static DataTable GetDataTable(DBconnector db)
        {
            DataTable result = new DataTable();
            if (db.DBType == "SQLite")
            {
                if (string.IsNullOrEmpty(db.DBConnectionString))
                {
                    throw new Exception("Database connection string cannot be empty");
                }
                using (SQLiteConnection connection = new SQLiteConnection(db.DBConnectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = db.DBCommand;
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            result.Load(reader);
                        }
                    }
                    connection.Close();
                }
            }
            else
            {
                throw new Exception("Database type not supported");
            }
            return result;
        }

        public static void InsertData(DBconnector db)
        {
            if (db.DBType == "SQLite")
            {
                if (string.IsNullOrEmpty(db.DBConnectionString))
                {
                    throw new Exception("Database connection string cannot be empty");
                }
                using (SQLiteConnection connection = new SQLiteConnection(db.DBConnectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = db.DBCommand;
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            else
            {
                throw new Exception("Database type not supported");
            }
        }

        public static int GetNextID(DBconnector db)
        {
            if (db.DBType == "SQLite")
            {
                if (string.IsNullOrEmpty(db.DBConnectionString))
                {
                    throw new Exception("Database connection string cannot be empty");
                }
                using (SQLiteConnection connection = new SQLiteConnection(db.DBConnectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "SELECT COUNT(*) FROM Tasks";
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return reader.GetInt32(0); // This will return 0 for the first task, 1 for the second, etc.
                            }
                        }
                    }
                    connection.Close();
                }
            }
            else
            {
                throw new Exception("Database type not supported");
            }
            return 0; // Default to 0 if no tasks exist
        }

        public static List<TTasks.TTask> GetTasks(DBconnector db)
        {
            if (db.DBType != "SQLite")
                throw new NotSupportedException($"Database type {db.DBType} not supported");

            var tasks = new List<TTasks.TTask>();

            using var connection = new SQLiteConnection(db.DBConnectionString);
            connection.Open();

            using var command = new SQLiteCommand("SELECT * FROM Tasks", connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                tasks.Add(new TTasks.TTask
                {
                    Name = reader["Name"]?.ToString() ?? string.Empty,
                    Description = reader["Description"]?.ToString() ?? string.Empty,
                    Status = reader["Status"]?.ToString() ?? string.Empty,
                    Priority = reader["Priority"]?.ToString() ?? string.Empty,
                    DueDate = reader["DueDate"]?.ToString() ?? string.Empty,
                    CreatedDate = reader["CreatedDate"]?.ToString() ?? string.Empty,
                    CompletedDate = reader["CompletedDate"]?.ToString() ?? string.Empty,
                    ID = reader["ID"]?.ToString() ?? string.Empty
                });
            }

            return tasks;
        }

        public static async Task<List<TTasks.TTask>> GetTasksAsync(DBconnector db)
        {
            if (db.DBType != "SQLite")
                throw new NotSupportedException($"Database type {db.DBType} not supported");

            var tasks = new List<TTasks.TTask>();

            using var connection = new SQLiteConnection(db.DBConnectionString);
            await connection.OpenAsync();
            using var command = new SQLiteCommand("SELECT * FROM Tasks", connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                tasks.Add(new TTasks.TTask
                {
                    Name = reader["Name"]?.ToString() ?? string.Empty,
                    Description = reader["Description"]?.ToString() ?? string.Empty,
                    Status = reader["Status"]?.ToString() ?? string.Empty,
                    Priority = reader["Priority"]?.ToString() ?? string.Empty,
                    DueDate = reader["DueDate"]?.ToString() ?? string.Empty,
                    CreatedDate = reader["CreatedDate"]?.ToString() ?? string.Empty,
                    CompletedDate = reader["CompletedDate"]?.ToString() ?? string.Empty,
                    ID = reader["ID"]?.ToString() ?? string.Empty
                });
            }

            return tasks;
        }

        public static int CreateTable(DBconnector db)
        {
            int result = 0;
            if (db.DBType == "SQLite")
            {
                if (string.IsNullOrEmpty(db.DBConnectionString))
                {
                    throw new Exception("Database connection string cannot be empty");
                }
                using (SQLiteConnection connection = new SQLiteConnection(db.DBConnectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = db.DBCommand;
                        result = command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            else
            {
                throw new Exception("Database type not supported");
            }
            return result;
        }

        public static async Task<bool> CreateTableAsync(DBconnector db)
        {
            if (db.DBType != "SQLite")
                throw new NotSupportedException($"Database type {db.DBType} not supported");

            if (string.IsNullOrEmpty(db.DBConnectionString))
                throw new Exception("Database connection string cannot be empty");

            using var connection = new SQLiteConnection(db.DBConnectionString);
            await connection.OpenAsync();
            using var command = new SQLiteCommand(db.DBCommand, connection);
            await command.ExecuteNonQueryAsync();
            return true;
        }
    }
}