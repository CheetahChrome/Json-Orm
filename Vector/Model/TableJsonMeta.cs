using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json.Orm.Model
{
    public class TableJsonMeta
    {
        public string? Schema { get; set; }
        public string? TableName { get; set; }
        public string Sql => $"select top(1) * from {(string.IsNullOrEmpty(Schema) ? string.Empty : Schema)}.{(string.IsNullOrEmpty(TableName) ? string.Empty : TableName)} for json auto;";
        public string? Json { get; set; }

        public bool HasData => !string.IsNullOrEmpty(Json);

        public TableJsonMeta(string tableName, string schema = "dbo")
        {
            Schema = schema;
            TableName = tableName;
        }
    }
}
