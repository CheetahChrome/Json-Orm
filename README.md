# Json-Orm
JSON Based ORM for SQL Server to .Net application. To achieve this, we use the feature to acquire `Get` data from SQL Server as JSON instead of tables. 
This is possible since version 2016 of SQL Server to generate JSON result sets. From those result sets we create models in our code and use the Json-Orm
helpers to provide list of models to our application.

Simply said, this package acts as an ORM, an *Object Relational Manager* between a .Net application and SQL Server. 

## Requirements
 1. One will need access to the database to be able to add Stored Procedures.
 2. Be able to create models based on database result. Usage of online converters to convert Json data to C# classes can be used or done by hand.
 3. To Post Data one will need to be able to create User Defined Table Types in SQL Server. *It's not hard once you get the hang of it.*

## Example From C#

From the Microsoft [Worldwide Importers Database](https://docs.microsoft.com/en-us/sql/samples/wide-world-importers-what-is) example database let us pull down all the cities using [Linqpad](https://www.linqpad.net/):

    var connectionStr = @"Data Source=.\Jabberwocky;Initial Catalog=WideWorldImporters";

    var jdb = new JsonOrmDatabase(connectionStr);

    var cities = jdb.Get<City>();

    cities.Dump();

How do we achieve that? Follow these steps detailed below.

---

# Steps to Fully Use Json-Orm

### Step 1 - Create Stored Procedure

We want to return data from `City` table in JSON form. For that we will create a stored procedure named `GetCities`. Let us return just 25 cities from the table in our example:

    CREATE PROCEDURE GetCities
    AS
    
    select Top 25 CityID, CityName, StateProvinceID
    from Application.Cities
    for json auto
    
    RETURN @@ROWCOUNT -- ORM does not use this value FYI
    
### Step 2 Execute Stored Procedure and Return Raw JSON
    
We will execute the stored procedure and look at the JSON result. One can do that by using a tool such as SSMS or AzureData Studio; most likely the tool you used to create the sproc.
*Note* -> We can also use the Json-Orm method `GetRawSQL` that returns the *raw* json as a string. This method is only used for debug/building purposes and is not directly tied to the standard Json-Orm processing. Here is our Linqpad Example:

    
    var connectionStr = @"Data Source=.\Jabberwocky;Integrated Security=SSPI;Initial Catalog=WideWorldImporters";

    var json = await JsonOrmDatabase.Create(connectionStr)
                                    .SetStoredProcedure("[get].[Cities]")
                                    .Execute();

    Console.Writeline(json);
    
    
--------    

## Moving Json to a Model (Deserialization)

For our purposes we will take the raw sql and remove all cities except for the first one to be used for step 3.

    [
      {
        "CityID": 1,
        "CityName": "Aaronsburg",
        "StateProvinceID": 39
      }
    ]
    
 ## Step 3 Create The Model
  
 Using an external tool or by hand we create a model which represents the json which will be returned from our Stored Procedure. Our model looks like this:

    public class City
    {
        public int CityID { get; set; }
        public string CityName { get; set; }
        public int StateProvinceID { get; set; }  
    }

  #### Step 3A
  
  If you haven't already add the Nuget Package `Json-Orm` to your project.
  
  ## Step 4 Modify Model To Be Json-Orm Specific
  
  There are two steps to achieve downloaded data from the server and marry it to a list of models. 
  
  #### Step 4A Inheirit Base Class `JsonOrmModel`
  
  See `City` below for that step.
  
  #### Step 4B Override `GetStoredProcedureName` Method
  
  The magic happens when the Json-Orm reflects on your model and uses that to call the Stored Procedures when `Get`-ing and `Push`-ing data. We want to call the *get* operation 
  and we override the string `GetStoredProcedureName` with our sproc name "[get].[Cities]". Currently our model looks like this:

    public class City : JsonOrmModel
    {
        public override string GetStoredProcedureName => "[get].[Cities]";
    
        public int CityID { get; set; }
        public string CityName { get; set; }
        public int StateProvinceID { get; set; }
    }
  
## Step 5 Get The Data
  
Believe it or not we are done, all we have to do is make the call to the database using an instance of the class `JsonOrmDatabase`. Here is the code in LinqPad:


     var connectionStr = @"Data Source=.\Jabberwocky;Initial Catalog=WideWorldImporters";

     var jdb = new JsonOrmDatabase(connectionStr);

     var cities = jdb.Get<City>();

     cities.Dump();
  
 ---------

# Coda

We have shown you how to get data from Sql Server, the Json-Orm tool also can post data to the database but that uses User Defined Table types and is explained more in the wiki. (Documentation not in Wiki Yet).
  
  
  
  
