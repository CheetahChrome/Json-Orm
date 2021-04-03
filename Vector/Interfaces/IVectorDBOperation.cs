using System;
using System.Collections.Generic;
using System.Text;

namespace Json.Orm.Interfaces
{
    public class IVectorDBOperation
    {
        string DBSprocName { get; }

        string Error { get; set; }

        bool HasError { get; }

        // Provides the ability to load during generic instantiation.
        // Usually used during one value instantiation and not JSON
        // deserialization.
        Action<object> Load { get; }

    }

}
