using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gianos.UniLib
{
    public class FieldInformation
    {
        public string tableName { get; private set; }
        public string fieldName { get; private set; }
        public string sqlType { get; set; }
        public string slxType { get; set; }

        public FieldInformation(string tableName, string fieldName)
        {
            this.tableName = tableName;
            this.fieldName = fieldName;
        }
    }
}
