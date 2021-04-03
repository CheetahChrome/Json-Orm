using System;
using System.Collections.Generic;
using System.Text;

namespace JSON_Vector.Attributes
{
    /// <summary>
    /// Used for table type properties. They need to be in a specific order to be vectored to the database.
    /// This attribute identifies what order the properties should be in, 1, 2, 3 etc as it builds
    /// the table type object to send to the database.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TableTypeAttribute : System.Attribute
    {
        public int ColumnIndex { get; set; }

        /// <summary>Allow a consumer to specify the index of the property to go into the sql TableType.</summary>
        public TableTypeAttribute(int columnIndex) => ColumnIndex = columnIndex;

    }
}
