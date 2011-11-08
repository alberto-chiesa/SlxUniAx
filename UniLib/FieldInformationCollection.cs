using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gianos.UniLib
{
    /// <summary>
    /// Class containing Informations about every field in the DB
    /// </summary>
    public class FieldInformationCollection
    {
        private Dictionary<string, Dictionary<string, FieldInformation>> tables;

        public FieldInformationCollection()
        {
            this.tables = new Dictionary<string,Dictionary<string,FieldInformation>>();
        }

        /// <summary>
        /// Get field information about a table
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <returns></returns>
        public Dictionary<string, FieldInformation> this[string tableName]
        {
            get
            {
                if (String.IsNullOrEmpty(tableName))
                    throw new ArgumentNullException("Table Name cannot be null or empty");

                tableName = tableName.ToUpper();

                if (!tables.ContainsKey(tableName))
                    throw new System.Collections.Generic.KeyNotFoundException(
                        String.Format("Table {0} not found.", tableName)
                    );

                return tables[tableName];
            }
        }

        public String[] GetTablesList()
        {
            List<String> tablesList = new List<string>();

            foreach (string tableName in this.tables.Keys)
            {
                var t = this.tables[tableName];

                foreach (var fieldName in t.Keys)
                {
                    // I need at least a field which is on the db as varchar or nvarchar AND
                    // which is a Text or UnicodeText in the model
                    // I do this check watching to slxType and sqlType
                    if (
                            (!String.IsNullOrEmpty(t[fieldName].slxType))
                            &&
                            (!String.IsNullOrEmpty(t[fieldName].sqlType))
                        )
                    {
                        tablesList.Add(tableName);
                        break;
                    }
                }
            }

            return tablesList.ToArray();
        }

        /// <summary>
        /// Searches and inits a field into the tables/fields collection
        /// </summary>
        /// <param name="tableName">The name of the db table</param>
        /// <param name="fieldName">The name of the db field</param>
        /// <returns></returns>
        public FieldInformation InitField(string tableName, string fieldName)
        {
            FieldInformation field = null;
            Dictionary<string, FieldInformation> fields;

            if (String.IsNullOrEmpty(tableName))
                throw new System.Exception("Table Name cannot be empty!");

            if (String.IsNullOrEmpty(fieldName))
                throw new System.Exception("Table Name cannot be empty!");

            tableName = tableName.ToUpper();
            fieldName = fieldName.ToUpper();

            // Check for table existence
            if (tables.ContainsKey(tableName.ToUpper()))
            {
                fields = tables[tableName];
            }
            else
            {
                fields = new Dictionary<string, FieldInformation>();
                tables[tableName] = fields;
            }

            // Check for field existence
            if (fields.ContainsKey(fieldName))
            {
                field = fields[fieldName];
            }
            else
            {
                field = new FieldInformation(tableName, fieldName);
                fields[fieldName] = field;
            }

            return field;
        }

        public FieldAction[] GetActions()
        {
            var actions = new List<FieldAction>();
            foreach(var tableName in this.tables.Keys)
            {
                var table = tables[tableName];

                foreach (var fieldName in table.Keys)
                {
                    var field = table[fieldName];

                    if ((field.NewState != FieldState.Unspecified)
                        &&
                        (field.State != field.NewState))
                    {
                        actions.Add(new FieldAction()
                        {
                            TableName = tableName,
                            FieldName = fieldName,
                            NewState = field.NewState,
                            FieldInfo = field
                        });
                    }
                }
            }

            return actions.ToArray();
        }
    }
}
