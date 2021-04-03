using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Json.Orm.Interfaces
{
    public interface IDBJsonAccess
    {
        Task<string> AcquireJson(string sproc, params SqlParameter[] parameters);

        Task<string> AcquireJson<T>() where T : IVectorDBOperation;

        string CallSProcReturnJSON(string sprocName, IEnumerable<SqlParameter> parameters);

        //      Task<List<T>> AcquireModels<T>(string sproc, params SqlParameter[] parameters) where T : IDbOperational;

    }
}
