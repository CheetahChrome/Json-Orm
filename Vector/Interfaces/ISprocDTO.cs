namespace JSON_Vector.Interfaces
{
    public interface ISprocDTO
    {
        /// <summary>
        /// This sproc will be used when getting the instances.
        /// </summary>
        string GetStoredProcedureName { get; }

        /// <summary>
        /// This sproc will be used when putting (usually Table Type objects) 
        /// </summary>
        string PutStoredProcedureName { get; }

        /// <summary>
        /// Turn this flag on when the alternate sproc should be used instead of the StoredProcedureName
        /// </summary>

        bool UseAlternateSproc { get; }

        /// <summary>
        /// Is the sproc being used, uses table types instead
        /// of just raw parameters?
        /// </summary>
        bool UsesTableTypes { get; }

    }
}