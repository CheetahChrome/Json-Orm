using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Json.Orm.Interfaces
{
    public interface IDBJsonAccess
    {
        Task<string> Get(string sproc, params SqlParameter[] parameters);

        Task<string> Get<T>() where T : IVectorDBOperation;

        string CallSProcReturnJSON(string sprocName, IEnumerable<SqlParameter> parameters);

        //      Task<List<T>> AcquireModels<T>(string sproc, params SqlParameter[] parameters) where T : IDbOperational;

    }
}
