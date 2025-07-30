using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data;

namespace Proyecto_Planilla_DSWI.Data
{
    public class ADOConnection
    {
        private static string _connectionString;

        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    // Obtener la cadena de conexión desde la configuración
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();

                    _connectionString = configuration.GetConnectionString("DefaultConnection");
                }
                return _connectionString;
            }
        }

        public static int ExecuteInt(string sql, MySqlParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = commandType;

                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    connection.Open();
                    var result = command.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        public static string ExecuteString(string sql, MySqlParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = commandType;

                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    connection.Open();
                    var result = command.ExecuteScalar();
                    return result?.ToString() ?? string.Empty;
                }
            }
        }

        public static bool ExecuteBool(string sql, MySqlParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = commandType;

                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    connection.Open();
                    var result = command.ExecuteScalar();
                    return result != null && Convert.ToBoolean(result);
                }
            }
        }

        public static DateTime ExecuteDateTime(string sql, MySqlParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = commandType;

                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    connection.Open();
                    var result = command.ExecuteScalar();
                    return result != null ? Convert.ToDateTime(result) : DateTime.MinValue;
                }
            }
        }

        public static DataTable ExecuteDataTable(string sql, MySqlParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = commandType;

                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    using (var adapter = new MySqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        public static MySqlDataReader ExecuteReader(string sql, MySqlParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            var connection = new MySqlConnection(ConnectionString);
            var command = new MySqlCommand(sql, connection);
            command.CommandType = commandType;

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            connection.Open();
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public static bool ExecuteNonQuery(string sql, MySqlParameter[] parameters = null, CommandType commandType = CommandType.Text)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.CommandType = commandType;

                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public static bool ExecuteTransaction(string[] sqlCommands, MySqlParameter[][] parameters = null)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        for (int i = 0; i < sqlCommands.Length; i++)
                        {
                            using (var command = new MySqlCommand(sqlCommands[i], connection, transaction))
                            {
                                if (parameters != null && parameters.Length > i && parameters[i] != null)
                                {
                                    command.Parameters.AddRange(parameters[i]);
                                }
                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public static T MapDataRowToObject<T>(DataRow row) where T : new()
        {
            var obj = new T();
            var type = typeof(T);
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                if (row.Table.Columns.Contains(property.Name) && row[property.Name] != DBNull.Value)
                {
                    try
                    {
                        var value = Convert.ChangeType(row[property.Name], property.PropertyType);
                        property.SetValue(obj, value);
                    }
                    catch
                    {
                        // Ignorar propiedades que no se pueden convertir
                    }
                }
            }

            return obj;
        }

        public static List<T> MapDataTableToList<T>(DataTable dataTable) where T : new()
        {
            var list = new List<T>();

            foreach (DataRow row in dataTable.Rows)
            {
                list.Add(MapDataRowToObject<T>(row));
            }

            return list;
        }



       }
} 