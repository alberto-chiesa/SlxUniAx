using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gianos.UniLib
{
    public enum FieldState
    {
        Unspecified,
        Unicode,
        Ansi,
        Error
    }

    public class FieldInformation
    {
        public string tableName { get; private set; }
        public string fieldName { get; private set; }
        public string sqlType { get; set; }
        public int sqlLength { get; set; }
        public string slxType { get; set; }
        public string slxLength { get; set; }

        public FieldInformation(string tableName, string fieldName)
        {
            this.tableName = tableName;
            this.fieldName = fieldName;
            this.NewState = FieldState.Unspecified;
        }

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

        private bool? isSlxUnicode
        {
            get
            {
                if (String.IsNullOrEmpty(slxType))
                    return null;

                if (slxType.Equals("UnicodeTextDataType", StringComparison.InvariantCultureIgnoreCase))
                    return true;
                if (sqlType.Equals("TextDataType", StringComparison.InvariantCultureIgnoreCase))
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
                bool? sql = this.isSqlUnicode;
                bool? slx = this.isSlxUnicode;

                if (sql == true && slx == true)
                    return FieldState.Unicode;

                if (sql == false && slx == false)
                    return FieldState.Ansi;

                return FieldState.Error;
            }
        }

        /// <summary>
        /// Gets Or Sets the New state for 
        /// </summary>
        public FieldState NewState { get; set; }
    }
}
