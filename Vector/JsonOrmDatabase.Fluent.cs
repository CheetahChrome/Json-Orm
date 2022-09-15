using Json.Orm.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Json.Orm
{
    public partial class JsonOrmDatabase
    {
        
        private List<SqlParameter> _SqlParameters;
        public List<SqlParameter> SqlParameters => _SqlParameters ??= new List<SqlParameter>();

        public bool HasParameters => (_SqlParameters != null && _SqlParameters.Any());

        /// <summary>
        /// This is the stored procedure to call if the fluent calls are used.
        /// </summary>
        public string StoredProcedure { get; set; }

        /// <summary>
        /// This is the result json if the fluent calls are used.
        /// </summary>
        public string Json { get; set; }

        public static JsonOrmDatabase Initialize(string connectionString)
            => Create(connectionString);

        public JsonOrmDatabase SetConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
            return this;
        }

        #region Parameters
        public JsonOrmDatabase AddParameterBoolean(string parameterName, bool value)
        {
            SqlParameters.Add(new SqlParameter()
            {
                ParameterName = parameterName,
                IsNullable = false,
                SqlDbType = System.Data.SqlDbType.Bit,
                Value = value ? 1 : 0,
            });

            return this;
        }

        public JsonOrmDatabase AddParameterInt(string parameterName, int value)
        {
            SqlParameters.Add(new SqlParameter()
            {
                ParameterName = parameterName,
                IsNullable = false,
                SqlDbType = System.Data.SqlDbType.Int,
                Value = value
            });

            return this;
        }

        public JsonOrmDatabase AddParameterSmallInt(string parameterName, int value)
        {
            SqlParameters.Add(new SqlParameter()
            {
                ParameterName = parameterName,
                IsNullable = false,
                SqlDbType = System.Data.SqlDbType.Int,
                Value = value
            });

            return this;
        }

        public JsonOrmDatabase AddParameterDate(string parameterName, DateTime value)
        {
            SqlParameters.Add(new SqlParameter()
            {
                ParameterName = parameterName,
                IsNullable = false,
                SqlDbType = System.Data.SqlDbType.Date,
                Value = value
            });

            return this;
        }
        public JsonOrmDatabase AddParameterDateTime(string parameterName, DateTime value)
        {
            SqlParameters.Add(new SqlParameter()
            {
                ParameterName = parameterName,
                IsNullable = false,
                SqlDbType = System.Data.SqlDbType.DateTime,
                Value = value
            });

            return this;
        }
        public JsonOrmDatabase AddParameterDateTime2(string parameterName, DateTime value)
        {
            SqlParameters.Add(new SqlParameter()
            {
                ParameterName = parameterName,
                IsNullable = false,
                SqlDbType = System.Data.SqlDbType.DateTime2,
                Value = value
            });

            return this;
        }


        public JsonOrmDatabase AddParameterDecimal(string parameterName, Decimal value)
        {
            SqlParameters.Add(new SqlParameter()
            {
                ParameterName = parameterName,
                IsNullable = false,
                SqlDbType = System.Data.SqlDbType.Decimal,
                Value = value
            });

            return this;
        }
        public JsonOrmDatabase AddParameterTime(string parameterName, DateTime value)
        {
            SqlParameters.Add(new SqlParameter()
            {
                ParameterName = parameterName,
                IsNullable = false,
                SqlDbType = System.Data.SqlDbType.Time,
                Value = value
            });

            return this;
        }


        public JsonOrmDatabase AddParameterNVarChar(string parameterName, string value)
        {
            SqlParameters.Add(new SqlParameter()
            {
                ParameterName = parameterName,
                IsNullable = false,
                SqlDbType = System.Data.SqlDbType.NVarChar,
                Value = value
            });

            return this;
        }


        public JsonOrmDatabase AddParameterUniqueIdentifier(string parameterName, Guid value)
        {
            SqlParameters.Add(new SqlParameter()
            {
                ParameterName = parameterName,
                IsNullable = false,
                SqlDbType = System.Data.SqlDbType.UniqueIdentifier,
                Value = value
            });

            return this;
        }

        public JsonOrmDatabase AddParameterXml(string parameterName, Guid value)
        {
            SqlParameters.Add(new SqlParameter()
            {
                ParameterName = parameterName,
                IsNullable = false,
                SqlDbType = System.Data.SqlDbType.Xml,
                Value = value
            });

            return this;
        }

        public JsonOrmDatabase AddParameterVarient(string parameterName, object value)
        {
            SqlParameters.Add(new SqlParameter()
            {
                ParameterName = parameterName,
                IsNullable = false,
                SqlDbType = System.Data.SqlDbType.Variant,
                Value = value
            });

            return this;
        }

        public JsonOrmDatabase AddParameterTableType<T>(string parameterName, T itemToSend) where T : class, ISprocOperationDTO, new()
        {
            SqlParameters.AddRange(itemToSend.Ex
        }

        #endregion

        public JsonOrmDatabase SetStoredProcedure(string storedProcedureName)
        {
            StoredProcedure = storedProcedureName;
            return this;
        }

        /// <summary>
        /// Execute allows one to run the operation and receive a raw version of json return in a string.
        /// </summary>
        /// <returns>String result of the operation.</returns>
        public async Task<string> Execute()
        {
#pragma warning disable CS8604 // Possible null reference argument.
            return await GetRawSQLAsync(StoredProcedure, HasParameters ? SqlParameters.ToArray() : null);
#pragma warning restore CS8604 // Possible null reference argument.

        }

        public async Task<T?> GetJsonAsync<T>(JsonSerializerOptions? options = null)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            Json = await Execute();
#pragma warning restore CS8604 // Possible null reference argument.

            return DeserializeJsonTo<T>(options);
        }

        /// <summary>
        /// Generic process the result and return an object(s) of the desired type.
        /// </summary>
        /// <typeparam name="T">Type to use</typeparam>
        /// <param name="options">JSON serializer options to adhere to</param>
        /// <returns></returns>
        public T? DeserializeJsonTo<T>(JsonSerializerOptions? options = null) 
            => JsonSerializer.Deserialize<T>(Json, options);

    }
}
