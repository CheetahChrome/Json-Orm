using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using Json.Orm.Attributes;
using Json.Orm.Extensions;
using Json.Orm.Interfaces;

namespace Json.Orm
{

    /// <summary>
    /// This is the common base class which allows derived models to be specialized vectors
    /// to and from the database. The derived class will overide these properties
    /// <code>
    ///   public override string StoredProcedureName => "[{dbSchema}].[{Stored Procedure Name}]";
    ///   public override string TableTypeName => "budgetTT"; // This is not a table type, this does not have to be filled in.
    /// </code>
    /// 
    /// To have the derived model mimic the table data, each property will be numbers from 1..n such as:
    /// <code>
    /// [TableType(1)]
    /// public int EngagementId { get; set; }
    /// 
    /// [TableType(2)]
    /// public string FacilityNumber { get; set; }
    /// 
    /// [TableType(3)]
    /// public string FacilityName { get; set; }
    /// </code>
    /// 
    /// Mark any non serialization needed property with
    /// <code>
    ///   [NotMapped]  // Ignored for database mapping (to db)
    ///   [JsonIgnore] // Ignred for any deserialization/serialization code.
    ///   public string SourceName { get; set; }
    /// </code>
    /// </summary>
    public class JsonOrmModel : ISprocOperationDTO, IValidation
    {

        #region Variables
        private List<string> _ValidationErrors;
        private Type NotMappedType = typeof(NotMappedAttribute);

        // The following two properties are to be overriden by the derived class.
        [NotMapped]
        public virtual string GetStoredProcedureName => string.Empty;

        [NotMapped]
        public virtual string PutStoredProcedureName => string.Empty;

        [NotMapped]
        public bool UseAlternateSproc { get; set; }

        /// <summary>
        /// This is an optional derived property which when used signifies that the derived is a tabletype and the
        /// name of that type.
        /// </summary>
        [NotMapped]
        public virtual string PutTableTypeVariableName => "TableTypeNameInvalid";

        /// <summary>
        /// Are we under debug mode? If so provide full error messages.
        /// </summary>
        [NotMapped]
        public bool IsDebug { get; set; }

        /// <summary>
        /// For production environments, do not send detailed error message, only a standard one.
        /// </summary>
        [NotMapped]
        public string SecureErrorMessage { get; set; }

        #endregion

        #region Properties
        [NotMapped]
        public string SourceName { get; set; }

        [NotMapped]
        public int Code { get; set; }
        #endregion

        public JsonOrmModel()
        {
            //IsDebug = Startup.ErrorMessageHandling.IsDebug;
            //SecureErrorMessage = Startup.ErrorMessageHandling.ValidationError;
        }

        #region Validation

        [NotMapped]
        public List<string> ValidationErrors { get => _ValidationErrors; set { } }

        /// <summary>
        /// Returns true if there are no validation issues.
        /// </summary>
        /// <returns>True when it is valid.</returns>
        public virtual bool Validate()
        {
            //if (sourceId < 1)
            //    AddValidationError("sourceId", "{empty}", "A source id must be provided");

            return (_ValidationErrors == null);
        }

        public void AddValidationError(string where, string what, string error)
        {
            if (_ValidationErrors == null)
                _ValidationErrors = new List<string>();

            if (IsDebug)
                _ValidationErrors.Add($"The field ({where}) is ({what}); validation reports error: {error}");
            else
            {
                if (!_ValidationErrors.Any())
                    _ValidationErrors.Add(SecureErrorMessage);
            }
        }

        public void SetValidationErrors(List<string> otherErrors)
        {
            if (otherErrors?.Any() ?? false)
            {
                if (_ValidationErrors == null)
                    _ValidationErrors = new List<string>();

                _ValidationErrors.AddRange(otherErrors);
            }

        }

        public string ToJSON()
        {
            var json = new GenericStatusResponse()
            {
                Responses = _ValidationErrors?.Select(error => new Response()
                {
                    HttpStatus = 400, // Invalid input
                    Message = error,
                    Severity = "Low",
                    Utc = DateTime.UtcNow
                })
                .ToList()
            };

            return json.ToJson();

        }


        #endregion

        #region StoredProcedure

        private List<PropertyInfo> _props;

        [NotMapped]
        public virtual bool UsesTableTypes => false;

        [NotMapped]
        protected List<PropertyInfo> GetProperties
        {
            get
            {
                if (_props == null)
                    _props = this.GetType()
                                 .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                 .ToList();

                return _props;

            }
        }


        public virtual List<Tuple<string, bool>> ExtractBools()
        {
            var booltype = typeof(bool);
            var booltypeNullable = typeof(bool?);

            return GetProperties.Where(prop => prop.PropertyType == booltype ||
                                              (prop.PropertyType == booltypeNullable
                                                   && prop.GetValue(this) != null))
                                .Where(prop => prop.CustomAttributes.Any(atr => atr.AttributeType == NotMappedType) == false)
                                .Select(prop => new Tuple<string, bool>(prop.Name, (bool)prop.GetValue(this)))
                                .ToList();
        }


        public virtual List<Tuple<string, DateTime>> ExtractDateTimes()
        {
            var dTtype = typeof(DateTime);
            var dTypeNullable = typeof(DateTime?);

            return GetProperties.Where(prop => prop.PropertyType == dTtype
                                              || (prop.PropertyType == dTypeNullable
                                                   && prop.GetValue(this) != null))
                                .Where(prop => prop.CustomAttributes.Any(atr => atr.AttributeType == NotMappedType) == false)
                                .Select(prop => new
                                {
                                    prop.Name,
                                    Value = prop.GetValue(this)
                                })
                                .Where(prop => ((DateTime)prop.Value).IsDateTimeValid())
                                .Select(prop => new Tuple<string, DateTime>(prop.Name, (DateTime)prop.Value))
                                .ToList();
        }


        public virtual List<Tuple<string, int>> ExtractInts()
        {
            var inttype = typeof(int);
            var intTypeNullable = typeof(int?);

            return GetProperties.Where(prop => prop.PropertyType == inttype
                                              || (prop.PropertyType == intTypeNullable
                                                   && prop.GetValue(this) != null))
                                .Where(prop => prop.CustomAttributes.Any(atr => atr.AttributeType == NotMappedType) == false)
                                .Select(prop => new
                                {
                                    prop.Name,
                                    Value = prop.GetValue(this)
                                })
                                .Where(prop => (int)prop.Value != 0)
                                .Select(prop => new Tuple<string, int>(prop.Name, (int)prop.Value))
                                .ToList();
        }

        public virtual List<Tuple<string, long>> ExtractLongs()
        {
            var longtype = typeof(long);
            var longTypeNullable = typeof(long?);

            return GetProperties.Where(prop => prop.PropertyType == longtype
                                              || (prop.PropertyType == longTypeNullable
                                                   && prop.GetValue(this) != null))
                                .Where(prop => prop.CustomAttributes.Any(atr => atr.GetType() == NotMappedType) == false)
                                .Select(prop => new
                                {
                                    prop.Name,
                                    Value = prop.GetValue(this)
                                })
                                .Where(prop => (long)prop.Value != 0)
                                .Select(prop => new Tuple<string, long>(prop.Name, (long)prop.Value))
                                .ToList();
        }

        public virtual List<Tuple<string, string>> ExtractStrings()
        {
            var strtype = typeof(string);

            return GetProperties.Where(prop => prop.PropertyType == strtype)
                                .Where(prop => prop.CustomAttributes.Any(atr => atr.AttributeType == NotMappedType) == false) // This will fail if something like `MaxLength` attribute is on it.
                                .Select(prop => new
                                {
                                    prop.Name,
                                    Value = prop.GetValue(this)?.ToString()
                                })
                                .Where(prop => string.IsNullOrWhiteSpace(prop.Value) == false)
                                .Select(prop => new Tuple<string, string>(prop.Name, prop.Value.ToString()))
                                .ToList();
        }


        /// <summary>
        /// Reflects this instance finding the `TableType` attributed properties
        /// and then creates an arrays datatables columns, such as
        ///  `Columns.Add("Name", typeof(string))`.
        ///  but to an array.
        /// </summary>
        /// <returns></returns>
        protected DataColumn[] ExtractDataColumnsOfTableType()
        {
            return GetProperties.Select(propInfo => new
            {
                index = propInfo.ExtractAttribute<TableTypeAttribute>()?.ColumnIndex ?? 0,
                propInfo
            })
                                .Where(p2 => p2.index > 0)
                                .Select(p2 => new DataColumn(p2.propInfo.Name, p2.propInfo.PropertyType))
                                .ToArray();
        }

        public virtual IEnumerable<SqlParameter> ExtractParameters()
        {
            yield return null;
        }

        // This is the final derivation of ExtractParameters which return a sql parameter object
        // which encompass the data in the generated table.
        public virtual SqlParameter ExtractParameters(bool ignoreValues)
        {
            return new SqlParameter()
            {
                ParameterName = $"@{PutTableTypeVariableName}",
                SqlDbType = SqlDbType.Structured,
                Value = ProcessValuesToTable(ExtractTable(), ignoreValues)
            };

        }

        /// <summary>
        /// Place each property into its own datable as a specific row. 
        /// </summary>
        /// <param name="table">Table to use, if null create a new table</param>
        /// <param name="ignoreValues">Ignore acquiring the actual values to be put in the row if this is an empty item to send.</param>
        /// <returns></returns>
        public virtual DataTable ProcessValuesToTable(DataTable table, bool ignoreValues = false)
        {

            // Enumerates this object finding the `TableType` attributed properties
            var ttProperties = GetProperties.Select(propInfo => new
            {
                index = propInfo.ExtractAttribute<TableTypeAttribute>()?.ColumnIndex ?? 0,
                propInfo
            })
                                            .Where(p2 => p2.index > 0)
                                            .OrderBy(p2 => p2.index)
                                            .Select(p2 => p2.propInfo)
                                            .ToList();

            // Don't add columns if the already exist when table is reused.
            if (table.Columns?.Count == 0)
            {
                // Set the columns
                // and adds them to the datatables columns, such as `Columns.Add("Name", typeof(string))`.
                table.Columns.AddRange(
                    ttProperties.Select(propInfo => new DataColumn(propInfo.Name,
                                                                   Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType))
                                .ToArray()
                    );
            }

            // We only ignore the adding of values when we are stubbing a call due to no data passed in by the user.
            if (ignoreValues == false)
            {
                // Set the data
                table.Rows.Add(
                    ttProperties.Select(propInfo => (object)propInfo.GetValue(this) ?? DBNull.Value)
                                .ToArray()
                    );
            }

            return table;

        }

        // This derivation allows for the chaining of data extraction 
        public DataTable ExtractTable()
        {
            return new DataTable(PutTableTypeVariableName);
        }

        /// <summary>
        /// Provides the ability for a common operation to
        /// share a datatable to insert multiple rows 
        /// </summary>
        /// <returns>The sql parameters to be sent to the database.</returns>
        public virtual SqlParameter GenerateSQLParameter(DataTable table)
        {
            return new SqlParameter()
            {
                ParameterName = $"@{PutTableTypeVariableName}",
                SqlDbType = SqlDbType.Structured,
                Value = table
            };
        }

        public SqlParameter ProcessThenYieldMultiples<T>(IList<T> items)
            where T : class, ISprocOperationDTO, new()
        {
            // When data in the list extract and process into a table and send back a sql parameter.
            if (items?.Any() ?? false)
            {
                var item = items.First();
                var table = item.ExtractTable();
                items.ToList()
                      .ForEach(itm => itm.ProcessValuesToTable(table, false));

                return item.GenerateSQLParameter(table);

            }
            else // Return a stub with no data
            {
                return Activator.CreateInstance<T>().ExtractParameters(true); ;
            }
        }

        #endregion

    }
}

