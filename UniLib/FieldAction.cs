using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gianos.UniLib
{
    public class FieldAction
    {
        public string FieldName { get; set; }
        public string TableName { get; set; }
        public FieldState NewState { get; set; }
        public FieldInformation FieldInfo { get; set; }

        public string ToString()
        {
            return TableName + "." + FieldName + " -> " + NewState.ToString();
        }
    }
}
