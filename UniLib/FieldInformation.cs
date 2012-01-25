using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gianos.UniLib
{
    /// <summary>
    /// Contains the possible state of a Field
    /// </summary>
    public enum FieldState
    {
        Unspecified,
        Unicode,
        Ansi,
        Error
    }

    

    public class FieldInformation
    {
        /// <summary>
        /// Field's table name.
        /// </summary>
        public string tableName { get; private set; }

        /// <summary>
        /// Field's name.
        /// </summary>
        public string fieldName { get; private set; }

        /// <summary>
        /// Type of the field on SQL (char, nvarchar, etc.)
        /// </summary>
        public string sqlType { get; set; }

        /// <summary>
        /// Length of the field on SQL
        /// </summary>
        public int sqlLength { get; set; }

        /// <summary>
        /// Type of the field on SLX, either UnicodeTextDataType or TextDataType
        /// </summary>
        public string slxType { get; set; }

        /// <summary>
        /// Length of the field on SalesLogix.
        /// </summary>
        public int slxLength { get; set; }

        /// <summary>
        /// Standard Constructor.
        /// </summary>
        /// <param name="tableName">The name of the TABLE of the field.</param>
        /// <param name="fieldName">The name of the FIELD on the db.</param>
        public FieldInformation(string tableName, string fieldName)
        {
            this.tableName = tableName;
            this.fieldName = fieldName;
            this.NewState = FieldState.Unspecified;
            this.NewLength = null;
        }

        /// <summary>
        /// Is this field Unicode Enabled on the Database? Returns null on error condition.
        /// </summary>
        private bool? isSqlUnicode
        {
            get
            {
                if (String.IsNullOrEmpty(sqlType))
                    return null;

                if (sqlType.Equals("nvarchar", StringComparison.InvariantCultureIgnoreCase)
                    ||
                    sqlType.Equals("nchar", StringComparison.InvariantCultureIgnoreCase))
                    return true;
                if (sqlType.Equals("varchar", StringComparison.InvariantCultureIgnoreCase)
                    ||
                    sqlType.Equals("char", StringComparison.InvariantCultureIgnoreCase))
                    return false;

                return null;
            }
        }

        /// <summary>
        /// Is this field Unicode Enabled on the Slx Model? Returns null on error condition.
        /// </summary>
        private bool? isSlxUnicode
        {
            get
            {
                if (String.IsNullOrEmpty(slxType))
                    return null;

                if (slxType.Equals("UnicodeTextDataType", StringComparison.InvariantCultureIgnoreCase))
                    return true;
                if (slxType.Equals("TextDataType", StringComparison.InvariantCultureIgnoreCase))
                    return false;

                return null;
            }
        }

        /// <summary>
        /// Gets the Field State:
        /// Unicode, Ansi or Error
        /// </summary>
        public FieldState State
        {
            get
            {
                // check db and model field's length
                if ((sqlLength != slxLength) ||
                    (sqlLength == 0) ||
                    (slxLength == 0))
                    return FieldState.Error;

                bool? sql = this.isSqlUnicode;
                bool? slx = this.isSlxUnicode;

                if (sql == true && slx == true)
                {
                    return FieldState.Unicode;
                }

                if (sql == false && slx == false)
                    return FieldState.Ansi;

                return FieldState.Error;
            }
        }

        /// <summary>
        /// Gets Or Sets the New state for 
        /// </summary>
        public FieldState NewState { get; set; }

        /// <summary>
        /// Gets or sets the new length for the field
        /// </summary>
        public int? NewLength { get; set; }

        /// <summary>
        /// ToString overridden method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            const string descriptionTemplate =
                @"{0}
{1}
{2}({3})
{4}({5})";
            return String.Format(descriptionTemplate,
                tableName,
                fieldName,
                slxType,
                slxLength,
                sqlType,
                sqlLength);
        }

        /// <summary>
        /// A Text field is a string for both sql and slx.
        /// </summary>
        public bool IsATextField
        {
            get
            {
                return !(String.IsNullOrEmpty(this.slxType) || String.IsNullOrEmpty(this.sqlType));
            }
        }

        /// <summary>
        /// Returns true if a New State has been specified
        /// or if the field should be resized
        /// </summary>
        public bool MustPerformAction
        {
            get
            {
                return (NewState != FieldState.Unspecified)
                    &&
                    ((State != NewState) || (NewLength > 0));
            }
        }
    }
}
