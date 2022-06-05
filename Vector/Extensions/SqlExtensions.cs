using Json.Orm.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json.Orm.Extensions
{
    public static class SQLExtensions
    {
        public static List<string?> GetTableNames(this SqlConnection connection, string tableSchema = "dbo")
        {
            using (connection)
            {
                connection.Open();

                return connection.GetSchema("Tables").Rows
                                 .Cast<DataRow>()
                                 .Where(row => row["TABLE_SCHEMA"].Equals(tableSchema))
                                 .Select(row => row["TABLE_NAME"].ToString())
                                 .OrderBy(nme => nme)
                                 .ToList();

            } // Auto Close exiting the block
        }

        public static List<TableJsonMeta> GetJsonForTables(this JsonOrmDatabase db,
                                                     string tableSchema = "dbo",
                                                     Func<string, bool> tableFilter = null)
        {
            

            var tables = db.Connection?.GetTableNames(tableSchema)
                                       .Where(nm => tableFilter?.Invoke(nm) ?? true)
                                       .Select(nm => new TableJsonMeta(nm, tableSchema))
                                       .ToList();

            if (!tables?.Any() ?? true)
                return new List<TableJsonMeta>();
            
            // Reset to a new connection.
            db.Connection = null;

            using (db.Connection)
            {
                db.Connection.Open();

                tables.ForEach(tbl => tbl.Json = db.GetRawSQLByDynamicSql(tbl.Sql));
            }

            return tables;

        }


    }
}
