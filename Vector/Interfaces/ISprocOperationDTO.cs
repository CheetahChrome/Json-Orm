using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Json.Orm.Interfaces
{
    public interface ISprocOperationDTO : ISprocDTO
    {
        //string StoredProcedureName { get; }

        // If the derived is a table type, then provide its name here. 
        string PutTableTypeVariableName { get; }

        List<Tuple<string, string>> ExtractStrings();

        List<Tuple<string, bool>> ExtractBools();

        List<Tuple<string, int>> ExtractInts();

        List<Tuple<string, long>> ExtractLongs();

        List<Tuple<string, DateTime>> ExtractDateTimes();

        ///// <summary>
        ///// Is the sproc being used, uses table types instead
        ///// of just raw parameters?
        ///// </summary>
        //bool UsesTableTypes { get; }

        // When dealing with multiples, provide a common table to use. 
        DataTable ExtractTable();

        /// <summary>
        /// Provides the ability for the DTO to generate
        /// its own parameters to send to the database.
        /// Usually used with sql table parameters.
        /// </summary>
        /// <returns>The sql parameters</returns>
        IEnumerable<SqlParameter> ExtractParameters();

        SqlParameter ExtractParameters(bool ignoreValues);

        /// <summary>
        /// Provides the ability for the DTO to generate
        /// its own parameters to send to the database.
        /// Usually used with sql table parameters.
        /// </summary>
        /// <returns>The sql parameters</returns>
        DataTable ProcessValuesToTable(DataTable table, bool ignoreValues = false);

        /// <summary>
        /// Provides the ability for a common operation to
        /// share a datatable to insert multiple rows 
        /// </summary>
        /// <returns>The sql parameters to be sent to the database.</returns>
        SqlParameter GenerateSQLParameter(DataTable table);


        /// <summary>
        /// Takes a list of items and generates a sqlparameter based on the instances themselves.
        /// </summary>
        /// <typeparam name="T">Type of class which adheres to the ISprocOperationDTO interface</typeparam>
        /// <param name="items">Actual instance items.</param>
        /// <returns>The sql parameters to be sent to the database.</returns>
        SqlParameter ProcessThenYieldMultiples<T>(IList<T> items) where T : class, ISprocOperationDTO, new();

    }


}