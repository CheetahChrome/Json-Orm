// See https://aka.ms/new-console-template for more information
using Json.Orm;
using TestConsole;

Console.WriteLine("Hello, World!");

var connectionStr = @"Data Source=.;Integrated Security=SSPI;Initial Catalog=Rasa";

//var jdb = new JsonOrmDatabase(connectionStr);

//string raw = jdb.GetRawSQL("[get].[Project]");

//Console.WriteLine(raw);
////raw.Dump();

//var pjc = jdb.Get<ProjectDTO>();

//var one = pjc[0];

//Console.WriteLine(one.Tpc);
//cities.Dump();


var result = await JsonOrmDatabase.Create(connectionStr)
                                  .SetStoredProcedure("[get].[PicklistMenus]")
                                  .GetJsonAsync<List<DetailCategory>>();



Console.WriteLine(result[0].Name);

