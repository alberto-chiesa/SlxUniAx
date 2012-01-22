using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gianos.UniLib
{
    public delegate void LogMethod(string message);

    /// <summary>
    /// Class containing Informations about every field in the DB
    /// </summary>
    public class FieldInformationManager
    {
        private Dictionary<string, Dictionary<string, FieldInformation>> tables;
        private DbHandler dbHandler;
        private SLXModelHandler slxModelHandler;

        private LogMethod Log;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FieldInformationManager()
        {
            this.tables = new Dictionary<string, Dictionary<string, FieldInformation>>();
            dbHandler = null;
            slxModelHandler = null;

            // no logger: log to console
            this.Log = delegate(string Msg) { Console.WriteLine(Msg); };
        }

        /// <summary>
        /// Constructor linking to a Logger method
        /// </summary>
        /// <param name="loggingMethod"></param>
        public FieldInformationManager(LogMethod loggingMethod)
        {
            this.Log = loggingMethod;
            this.tables = new Dictionary<string, Dictionary<string, FieldInformation>>();
            dbHandler = null;
            slxModelHandler = null;
        }

        /// <summary>
        /// Setup the connection to the database
        /// </summary>
        /// <param name="dbHandler"></param>
        public void LinkToDb(DbHandler dbHandler)
        {
            this.dbHandler = dbHandler;
        }

        /// <summary>
        /// Setup the connection to slx model
        /// </summary>
        /// <param name="slxModelHandler"></param>
        public void LinkToSlxModel(SLXModelHandler slxModelHandler)
        {
            this.slxModelHandler = slxModelHandler;
        }

        /// <summary>
        /// Reads information from both database and model
        /// </summary>
        public bool LoadFieldInformations()
        {
            this.tables = new Dictionary<string, Dictionary<string, FieldInformation>>();

            try
            {
                this.Log("Started metadata collection...");
                this.Log("");
                this.Log("Accessing db data...");

                this.dbHandler.ReadTableDataFromSLXDb(this);

                this.Log("done. Reading data from model...");

                this.slxModelHandler.FindEntityModels(this);
                this.Log("Done. Load completed succesfully!");
            }
            catch (Exception exc)
            {
                this.Log("Whoops. Seems something went wrong.");
                this.Log("Caught exception:");
                this.Log(exc.Message);

                return false;
            }

            return true;
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

        /// <summary>
        /// Returns a list of all the tables/entities in the system
        /// with text fields (strings in the entity, varchar/nvarchar in sql)
        /// </summary>
        /// <returns>Iterator of the tables</returns>
        public IEnumerable<String> GetTablesList()
        {
            // table.Value is dictionary of fields
            var ret = from table in this.tables
                      where table.Value.Any(field => field.Value.IsATextField)
                      select table.Key;

            return ret;
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
            if (!tables.ContainsKey(tableName))
                tables[tableName] = new Dictionary<string, FieldInformation>();

            fields = tables[tableName];
            
            // Check for field existence
            if (!fields.ContainsKey(fieldName))
                fields[fieldName] = new FieldInformation(tableName, fieldName);

            field = fields[fieldName];

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

                    if (
                            (field.NewState != FieldState.Unspecified)
                            &&
                            (
                                (field.State != field.NewState)
                                ||
                                (field.NewLength > 0)
                            )
                        )

                    {
                        actions.Add(new FieldAction()
                        {
                            TableName = tableName,
                            FieldName = fieldName,
                            NewState = field.NewState,
                            FieldInfo = field,
                            NewSize = field.NewLength ?? 0
                        });
                    }
                }
            }

            return actions.ToArray();
        }

        /// <summary>
        /// Executes the specified actions on the db and on slx model
        /// </summary>
        public void RunActions()
        {
            this.Log("Reading actions to be performed...");

            var actions = GetActions();

            this.Log(actions.Length + " found.");

            this.Log("Updating model...");
            this.slxModelHandler.ApplyActionsToModel(actions);

            this.Log("Updating database...");
            this.dbHandler.ApplyActionsToDb(actions);

            this.Log("");
            this.Log("Update Complete!");
        }
    }
}
