using System.Collections.Generic;

namespace JSON.ORM.Vector.Interfaces
{
    /// <summary>
    /// Provides a system to validate an incoming object from a consumer, but doing
    /// specific validation checks during object processing. 
    /// </summary>
    public interface IValidation
    {
        List<string> ValidationErrors { get; set; }

        bool Validate();

        int Code { get; set; }

        string ToJSON();

    }
}