// See https://aka.ms/new-console-template for more information
using Json.Orm;
using Json.Orm.Extensions;
using System.Data.SqlClient;
using TestConsole;

var connectionStr = @"Data Source=.;Integrated Security=SSPI;Initial Catalog=Rasa";

var jdb = new JsonOrmDatabase(connectionStr);

//string raw = jdb.GetRawSQL("[get].[Project]");

//Console.WriteLine(raw);
////raw.Dump();

//var pjc = jdb.Get<ProjectDTO>();

//var one = pjc[0];

//Console.WriteLine(one.Tpc);
//cities.Dump();

// Standard Fluent
//var result = await JsonOrmDatabase.Create(connectionStr)
//                                  .SetStoredProcedure("[get].[PicklistMenus]")
//                                  .GetJsonAsync<List<DetailCategory>>();

//Console.WriteLine(result[0].Name);

//  Fluent raw string json
var result = await JsonOrmDatabase.Create(connectionStr)
                                  .SetStoredProcedure("[get].[PicklistMenus]")
                                  .Execute();

Console.WriteLine(result);


// Raw SQL call

//var names = new SqlConnection(connectionStr).GetTableNames("info")
//                                            .Select(nm => $"select top(1) * from info.{nm} for json auto;")
//                                            .ToList();

//Console.WriteLine(string.Join(Environment.NewLine, names));

//var json = jdb.GetRawSQLByDynamicSql(names[10]);

//Console.WriteLine(json);


/* Get Table Names

var TableJsons = jdb.GetJsonForTables("info");

Console.WriteLine(TableJsons.Any() ? "True" : "False");

*/
