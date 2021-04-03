using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JSON_Vector
{
    public partial class GenericStatusResponse
    {
        public List<Response> Responses { get; set; }

        public string ToJson() => JsonSerializer.Serialize(this);

    }

    public partial class Response
    {
        public int? HttpStatus { get; set; }

        public string Severity { get; set; }

        public string Message { get; set; }

        public string Specific { get; set; }

        public DateTimeOffset Utc { get; set; }

        public string ToJson() => JsonSerializer.Serialize(this);

    }

}
