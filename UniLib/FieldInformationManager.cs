using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
            return from table in this.tables
                   where table.Value.Any(field => field.Value.IsATextField)
                   select table.Key;
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

        /// <summary>
        /// Get a list of actions from the field data.
        /// </summary>
        /// <returns></returns>
        public FieldAction[] GetActionArray()
        {
            return EnumerateActions().ToArray<FieldAction>();
        }

        /// <summary>
        /// Build a list of FieldAction(s) based on the
        /// field informations available
        /// </summary>
        /// <returns></returns>
        private IEnumerable<FieldAction> EnumerateActions()
        {
            return from table in this.tables
                   from field in table.Value
                   where field.Value.MustPerformAction
                   select new FieldAction()
                   {
                       TableName = table.Key,
                       FieldName = field.Key,
                       NewState = field.Value.NewState,
                       FieldInfo = field.Value,
                       NewSize = field.Value.NewLength ?? 0
                   };
        }

        /// <summary>
        /// Executes the specified actions on the db and on slx model
        /// </summary>
        public void RunActions()
        {
            this.Log("Reading actions to be performed...");

            var actions = GetActionArray();

            this.Log(actions.Length + " found.");

            this.Log("Updating model...");
            this.slxModelHandler.ApplyActionsToModel(actions);

            this.Log("Updating database...");
            this.dbHandler.ApplyActionsToDb(actions);

            this.Log("");
            this.Log("Update Complete!");
        }

        /// <summary>
        /// Builds a bundle reading the actions from the
        /// field data.
        /// A bundle is simply a list of action rows, each
        /// representing the ToString() of an action
        /// </summary>
        /// <returns>The bundle text</returns>
        public string GetBundleText()
        {
            var sb = new StringBuilder();

            var actionsByTable = from act in this.EnumerateActions()
                          group act by act.TableName into tableActions
                          orderby tableActions.Key
                          select new {
                              TableName = tableActions.Key,
                              TableActions = (from act in tableActions
                                             let toString = act.ToString()
                                             orderby toString
                                             select toString).ToList<String>()
                          };

            foreach (var tableActionsData in actionsByTable)
            {
                sb.AppendLine(String.Format("# Actions for {0} table", tableActionsData.TableName));
                
                foreach (var act in tableActionsData.TableActions)
                    sb.AppendLine(act);
                
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public void LoadAndApplyBundle(Stream stream)
        {
            try
            {
                StreamReader sr = new StreamReader(stream, UTF8Encoding.UTF8);
                string bundleText = sr.ReadToEnd();
                sr.Dispose();

                LoadAndApplyBundle(bundleText);
            }
            finally
            {
                if (stream != null) stream.Dispose();
            }
        }

        public void LoadAndApplyBundle(string bundleText)
        {
            var actions = FieldAction.Parse(bundleText);
            
            if (actions != null && actions.Count > 0)
                this.ApplyBundleToFieldInformation(actions);
        }

        /// <summary>
        /// Given a list of actions, applies them to the field data.
        /// Throws exceptions if inexistent fields are specified
        /// </summary>
        /// <param name="actions">Enumerable set of actions</param>
        public void ApplyBundleToFieldInformation(IEnumerable<FieldAction> actions)
        {
            foreach (var action in actions)
            {
                if (!this.tables.ContainsKey(action.TableName))
                    throw new ArgumentException(String.Format(
                        "Error loading bundle: {0} is not a valid table name.", action.TableName));

                var table = this.tables[action.TableName];
                if (!table.ContainsKey(action.FieldName))
                    throw new ArgumentException(String.Format(
                        "Error loading bundle: {0} is not a valid field name for table {1}.", action.FieldName, action.TableName));

                var field = table[action.FieldName];

                if (field.NewState != action.NewState || action.NewSize > 0)
                {
                    field.NewState = action.NewState;
                    if (action.NewSize > 0) field.NewLength = action.NewSize;
                }
            }
        }


        public void SaveBundleTo(System.IO.Stream bundleFile)
        {
            byte[] utf8EncodedBundle = UTF8Encoding.UTF8.GetBytes(GetBundleText());

            bundleFile.Write(utf8EncodedBundle, 0, utf8EncodedBundle.Length);
        }
    }
}
