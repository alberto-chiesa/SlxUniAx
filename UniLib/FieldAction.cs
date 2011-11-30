using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gianos.UniLib
{
    /// <summary>
    /// Class modeling an action to be performed on a field.
    /// </summary>
    public class FieldAction
    {
        /// <summary>
        /// Name of the table containing the field
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Name of the field to be updated
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Name of the State (Unicode or Text) to be set on the field
        /// </summary>
        public FieldState NewState { get; set; }

        /// <summary>
        /// New size to which the field must be set. 0 means "as is".
        /// </summary>
        public int NewSize { get; set; }

        /// <summary>
        /// Link to the Instance of the field information
        /// </summary>
        public FieldInformation FieldInfo { get; set; }

        public FieldAction()
        {
            this.TableName = this.FieldName = String.Empty;
            this.NewState = FieldState.Unspecified;
            this.NewSize = 0;
            this.FieldInfo = null;
        }

        /// <summary>
        /// To string method.
        /// </summary>
        /// <returns></returns>
        public string ToString()
        {
            if (String.IsNullOrEmpty(TableName) || String.IsNullOrEmpty(FieldName))
            {
                return String.Empty;
            }

            return (NewSize > 0) ?
                TableName + "." + FieldName + " -> " + NewState.ToString() + "(" + NewSize + ")" :
                TableName + "." + FieldName + " -> " + NewState.ToString();
        }
    }
}
