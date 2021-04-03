using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSON_Vector.Interfaces;
//using Microsoft.AspNetCore.Mvc;

namespace JSON_Vector.Connection
{
    /// <summary>
    /// Handle db interaction to get and post data.
    /// </summary>
//    public class JsonConnectionBase
//    {
//        public IDBJsonAccess Connection { get; set; }
//        public JsonConnectionBase(IDBJsonAccess connection) => Connection = connection;

//        public IActionResult Get(string sprocName)
//            => new ObjectResult(Connection.CallSProcReturnJSON(sprocName, null));

//        /// <summary>
//        /// The entity which adheres to ISprocOperationDTO will give us the sproc name to call as property `StoredProcedureName`.
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="entity"></param>
//        /// <returns></returns>
//        public IActionResult Post<T>(T entity) where T : ISprocOperationDTO
//        {
//            var pms = entity.ExtractParameters().ToList();
//            var json = Connection.CallSProcReturnJSON(entity.StoredProcedureName, pms);

            
//            return new ObjectResult(json);

//        }
//    }
}
