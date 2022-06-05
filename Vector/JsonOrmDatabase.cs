using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Json.Orm.Interfaces;
using SQLJSON.Extensions;

namespace Json.Orm
{
    /// <summary>
    /// This class encompasses sending class models to a database.
    /// The outgoing operations are performed by taking the data found in a list of model classes
    /// and sending up as sql server user table type values to sprocs. 
    /// </summary>
    /// <remarks>This code is extracted from the nuget package JSONVector and the code
    /// found in the repository github.com/CheetahChrome/JSON-Framework. The
    /// code is the MIT license and I am the original author (William Wegerson) of the code having
    /// worked on it before doing a project for Huron</remarks>
    public partial class JsonOrmDatabase : IDisposable
    {
        private bool disposedValue;
        private SqlConnection? _Connection;

        public string? ConnectionString { get; set; }

        public static JsonOrmDatabase Create()
            => new();

        public static JsonOrmDatabase Create(string connectionString)
            => new(connectionString);

        public SqlConnection? Connection
        {
            get => _Connection ??= new SqlConnection(ConnectionString);
            set => _Connection = value;
        }

        public JsonOrmDatabase() => ConnectionString = "";  

        /// <summary>
        /// Create a instance with the connection string.
        /// </summary>
        /// <param name="connectionString"></param>
        public JsonOrmDatabase(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void Post<T>(T itemTOSend, params SqlParameter[] parameters) where T : class, ISprocOperationDTO, new()
        {
            var sproc = itemTOSend.PutStoredProcedureName;

            if (string.IsNullOrWhiteSpace(sproc))
                throw new ApplicationException("List instance type does not contain a valid stored procedure name. Set in `PutStoredProcedureName` property.");

            var paramList = parameters.ToList();

            if (!paramList.Any())
                paramList.AddRange(itemTOSend.ExtractParameters());

            Connection = new SqlConnection(ConnectionString);
            {
                Connection.Open();

                try
                {
                    using (var cmd = new SqlCommand(sproc, Connection) { CommandType = CommandType.StoredProcedure, CommandTimeout = 600 })
                    {
                        if (paramList.Any())
                            cmd.Parameters.AddRange(paramList.ToArray());

                        _ = cmd.ExecuteNonQuery();

                        //                    result = await reader.ReadAllAsync();
                    }
                }
                catch (ArgumentException aex)
                {
                    throw new ApplicationException(
                        $"Database SQL Error Due To call failure to Stored Procedure. Report to Development.{Environment.NewLine}{Environment.NewLine}{sproc} :  {aex.Message}{Environment.NewLine}{Environment.NewLine}",
                        aex);
                }

                Connection.Close();
            }

            Connection = null;

        }

        /// <summary>
        /// Call a stored procedure using the type sproc target.
        /// </summary>
        /// <remarks>This method is best for Table Type (single variable, multiple items in the `table` of the type.) in sproc</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="listToSend"></param>
        public void Post<T>(IList<T> listToSend) where T : class, ISprocOperationDTO, new()
        {
            var list = listToSend ?? throw new ApplicationException("Need a valid list");
            if (!list.Any())
                throw new ApplicationException("The data list must contain at least one item.");

            var typeInstance = listToSend.First();

            var sproc = typeInstance.PutStoredProcedureName;

            if (string.IsNullOrWhiteSpace(sproc))
                throw new ApplicationException("List instance type does not contain a valid stored procedure name. Set in `StoredProcedureName` property.");

            // Distill the list of items into one parameter with an internal table.
            // Generally found in table types being sent.
            SqlParameter parameter = typeInstance.ProcessThenYieldMultiples(listToSend);

            Connection = new SqlConnection(ConnectionString);
            {
                Connection.Open();

                using (var cmd = new SqlCommand(sproc, Connection) { CommandType = CommandType.StoredProcedure, CommandTimeout = 600 })
                {
                    if (Connection != null)
                        cmd.Parameters.Add(parameter);

                    _ = cmd.ExecuteNonQuery();

                    //                    result = await reader.ReadAllAsync();
                }

                Connection?.Close();
            }

            Connection = null;

        }

        /// <summary>
        /// Call a stored procedure using the type sproc target.
        /// </summary>
        /// <remarks>This method is best for Table Type (single variable, multiple items in the `table` of the type.) in sproc</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="listToSend"></param>
        public async Task PostAsync<T>(IList<T> listToSend) where T : class, ISprocOperationDTO, new()
        {
            var list = listToSend ?? throw new ApplicationException("Need a valid list");
            if (!list.Any())
                throw new ApplicationException("The data list must contain at least one item.");

            var typeInstance = listToSend.First();

            var sproc = typeInstance.PutStoredProcedureName;

            if (string.IsNullOrWhiteSpace(sproc))
                throw new ApplicationException("List instance type does not contain a valid stored procedure name. Set in `StoredProcedureName` property.");

            // Distill the list of items into one parameter with an internal table.
            // Generally found in table types being sent.
            SqlParameter parameter = typeInstance.ProcessThenYieldMultiples(listToSend);

            Connection = new SqlConnection(ConnectionString);
            {
                Connection.Open();

                using (var cmd = new SqlCommand(sproc, Connection) { CommandType = CommandType.StoredProcedure, CommandTimeout = 600 })
                {
                    if (Connection != null)
                        cmd.Parameters.Add(parameter);

                    _ = await cmd.ExecuteNonQueryAsync();

                    //                    result = await reader.ReadAllAsync();
                }

                Connection?.Close();
            }

            Connection = null;

        }

        /// <summary>
        /// When passing a list of objects to the database.
        /// </summary>
        /// <remarks>Parameters is optional, otherwise the process will reflect on the item to send to the database to generate the parameters.</remarks>
        /// <typeparam name="T">Type of object being sent</typeparam>
        /// <param name="listToSend">The list of instances to send</param>
        /// <param name="parameters">Optional sql parameters if `rolling your own` parameters. See remarks above.</param>
        public void Post<T>(IList<T> listToSend, params SqlParameter[] parameters) where T : class, ISprocOperationDTO, new()
        {
            var list = listToSend ?? throw new ApplicationException("Need a valid list");
            if (!list.Any())
                throw new ApplicationException("The data list must contain at least one item.");

            var typeInstance = listToSend.First();

            var sproc = typeInstance.PutStoredProcedureName;

            if (string.IsNullOrWhiteSpace(sproc))
                throw new ApplicationException("List instance type does not contain a valid stored procedure name. Set in `StoredProcedureName` property.");

            var paramList = parameters.ToList();

            //     if (!paramList.Any())
            paramList.Add(typeInstance.ProcessThenYieldMultiples(listToSend));

            Connection = new SqlConnection(ConnectionString);
            {
                Connection.Open();

                try
                {
                    using (var cmd = new SqlCommand(sproc, Connection) { CommandType = CommandType.StoredProcedure, CommandTimeout = 600 })
                    {
                        if (paramList.Any())
                            cmd.Parameters.AddRange(paramList.ToArray());

                        _ = cmd.ExecuteNonQuery();

                        //                    result = await reader.ReadAllAsync();
                    }
                }
                catch (ArgumentException aex)
                {
                    throw new ApplicationException(
                        $"Database SQL Error Due To call failure to Stored Procedure. Report to Development.{Environment.NewLine}{Environment.NewLine}{sproc} :  {aex.Message}{Environment.NewLine}{Environment.NewLine}",
                        aex);
                }

                Connection.Close();
            }

            Connection = null;

        }

        /// <summary>
        /// When passing a list of objects to the database.
        /// </summary>
        /// <remarks>Parameters is optional, otherwise the process will reflect on the item to send to the database to generate the parameters.</remarks>
        /// <typeparam name="T">Type of object being sent</typeparam>
        /// <param name="listToSend">The list of instances to send</param>
        /// <param name="parameters">Optional sql parameters if `rolling your own` parameters. See remarks above.</param>
        public async Task PostAsync<T>(IList<T> listToSend, params SqlParameter[] parameters) where T : class, ISprocOperationDTO, new()
        {
            var list = listToSend ?? throw new ApplicationException("Need a valid list");
            if (!list.Any())
                throw new ApplicationException("The data list must contain at least one item.");

            var typeInstance = listToSend.First();

            var sproc = typeInstance.PutStoredProcedureName;

            if (string.IsNullOrWhiteSpace(sproc))
                throw new ApplicationException("List instance type does not contain a valid stored procedure name. Set in `StoredProcedureName` property.");

            var paramList = parameters.ToList();

            //     if (!paramList.Any())
            paramList.Add(typeInstance.ProcessThenYieldMultiples(listToSend));

            Connection = new SqlConnection(ConnectionString);
            {
                Connection.Open();

                try
                {
                    using (var cmd = new SqlCommand(sproc, Connection) { CommandType = CommandType.StoredProcedure, CommandTimeout = 600 })
                    {
                        if (paramList.Any())
                            cmd.Parameters.AddRange(paramList.ToArray());

                        _ = await cmd.ExecuteNonQueryAsync();

                        //                    result = await reader.ReadAllAsync();
                    }
                }
                catch (ArgumentException aex)
                {
                    throw new ApplicationException(
                        $"Database SQL Error Due To call failure to Stored Procedure. Report to Development.{Environment.NewLine}{Environment.NewLine}{sproc} :  {aex.Message}{Environment.NewLine}{Environment.NewLine}",
                        aex);
                }

                Connection.Close();
            }

            Connection = null;

        }

        public List<T> Get<T>(params SqlParameter[] parameters) where T : ISprocOperationDTO
        {
            var result = string.Empty;
            var instance = Activator.CreateInstance<T>();

            if (string.IsNullOrEmpty(instance.GetStoredProcedureName)) throw new ApplicationException("Development error, Type T does not have a `GetStoredProcedureName` override specifying the sproc to use");

            using (SqlConnection conn = new(ConnectionString))
            {
                conn.Open();
                try
                {
                    using (var cmd = new SqlCommand(instance.GetStoredProcedureName, conn)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 600
                    })
                    { 
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);

                        var reader = cmd.ExecuteJsonReader();

                        result = reader.ReadAll();
                    }
                }
                catch (SqlException sql)
                {
                    throw new ApplicationException(
                        $"Database SQL Error Due To Sproc or sql script internal processing issue. Report to Development.{Environment.NewLine}{Environment.NewLine}{instance.GetStoredProcedureName} :  {sql.Message}{Environment.NewLine}{Environment.NewLine}",
                        sql);
                }
                finally
                {
                    conn.Close();
                }
            }

            // Return the deserialized models, or an empty list. 
            return string.IsNullOrEmpty(result) ? new List<T>() 
                                                : JsonSerializer.Deserialize<List<T>>(result, new JsonSerializerOptions()
                                                { PropertyNameCaseInsensitive = true}) 

                                                ?? new List<T>();
        }


        public async Task<List<T>> GetAsync<T>(params SqlParameter[] parameters) where T : ISprocOperationDTO
        {
            var result = string.Empty;
            var instance = Activator.CreateInstance<T>();

            if (string.IsNullOrEmpty(instance.GetStoredProcedureName)) throw new ApplicationException("Development error, Type T does not have a `GetStoredProcedureName` override specifying the sproc to use");

            using (SqlConnection conn = new(ConnectionString))
            {
                conn.Open();
                try
                {
                    using (var cmd = new SqlCommand(instance.GetStoredProcedureName, conn)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 600
                    })
                    {
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);

                        var reader = await cmd.ExecuteJsonReaderAsync();

                        result = await reader.ReadAllAsync();
                    }
                }
                catch (SqlException sql)
                {
                    throw new ApplicationException(
                        $"Database SQL Error Due To Sproc or sql script internal processing issue. Report to Development.{Environment.NewLine}{Environment.NewLine}{instance.GetStoredProcedureName} :  {sql.Message}{Environment.NewLine}{Environment.NewLine}",
                        sql);
                }
                finally
                {
                    conn.Close();
                }
            }

            // Return the deserialized models, or an empty list. 
            return string.IsNullOrEmpty(result) ? new List<T>()
                                                : JsonSerializer.Deserialize<List<T>>(result, new JsonSerializerOptions()
                                                { PropertyNameCaseInsensitive = true })

                                                ?? new List<T>();
        }


        /// <summary>
        /// Used for debug purposes only, will execute a stored procedure and return the raw JSON for Model class building operations.
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure to execute</param>
        /// <returns>JSON returned or string.Empty if error.</returns>
        public string GetRawSQL(string storedProcedureName, params SqlParameter[] parameters)
        {
            string result = string.Empty;
            using SqlConnection conn = new(ConnectionString);

            conn.Open();

            try
            {
                using var cmd = new SqlCommand(storedProcedureName, conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 10
                };

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                var reader = cmd.ExecuteJsonReader();

                result = reader.ReadAll();
            }
            catch (SqlException sql)
            {
                throw new ApplicationException(
                    $"Database SQL Error Due To Sproc or sql script internal processing issue. Report to Development.{Environment.NewLine}{Environment.NewLine}{storedProcedureName} :  {sql.Message}{Environment.NewLine}{Environment.NewLine}",
                    sql);
            }
            finally
            {
                conn.Close();
            }

            return result;

        }

        /// <summary>
        /// Used for debug purposes only, will execute a stored procedure and return the raw JSON for Model class building operations.
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure to execute</param>
        /// <returns>JSON returned or string.Empty if error.</returns>
        public async Task<string> GetRawSQLAsync(string storedProcedureName, params SqlParameter[] parameters)
        {
            string result = string.Empty;
            using SqlConnection conn = new(ConnectionString);

            conn.Open();

            try
            {
                using var cmd = new SqlCommand(storedProcedureName, conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 10
                };

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                var reader = await cmd.ExecuteJsonReaderAsync();

                result = await reader.ReadAllAsync();
            }
            catch (SqlException sql)
            {
                throw new ApplicationException(
                    $"Database SQL Error Due To Sproc or sql script internal processing issue. Report to Development.{Environment.NewLine}{Environment.NewLine}{storedProcedureName} :  {sql.Message}{Environment.NewLine}{Environment.NewLine}",
                    sql);
            }
            finally
            {
                conn.Close();
            }

            return result;

        }



        public string GetRawSQLByDynamicSql(string sql, params SqlParameter[] parameters)
        {
            string result = string.Empty;
            using SqlConnection conn = new(ConnectionString);

            conn.Open();

            try
            {
                using var cmd = new SqlCommand(sql, conn)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 10
                };

                if ((parameters != null) && (parameters.Any()))
                    cmd.Parameters.AddRange(parameters);

                var reader = cmd.ExecuteJsonReader();

                result = reader.ReadAll();
            }
            catch (SqlException sqle)
            {
                throw new ApplicationException(
                    $"Database SQL Error Due To Sproc or sql script internal processing issue. Report to Development.{Environment.NewLine}{Environment.NewLine}{sql} :  {sqle.Message}{Environment.NewLine}{Environment.NewLine}",
                    sqle);
            }
            finally
            {
                conn.Close();
            }

            return result;
        }


        public async Task<string> GetRawSQLByDynamicSqlAsync(string sql, params SqlParameter[] parameters)
        {
            string result = string.Empty;
            using SqlConnection conn = new(ConnectionString);

            conn.Open();

            try
            {
                using var cmd = new SqlCommand(sql, conn)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = 10
                };

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                var reader = await cmd.ExecuteJsonReaderAsync();

                result = await reader.ReadAllAsync();
            }
            catch (SqlException sqle)
            {
                throw new ApplicationException(
                    $"Database SQL Error Due To Sproc or sql script internal processing issue. Report to Development.{Environment.NewLine}{Environment.NewLine}{sql} :  {sqle.Message}{Environment.NewLine}{Environment.NewLine}",
                    sqle);
            }
            finally
            {
                conn.Close();
            }

            return result;
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    Connection?.Close();
                    Connection = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~VectorDatabase()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}
