using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;

namespace Gianos.UniLib
{
    /// <summary>
    /// Class incapsulating database management methods
    /// </summary>
    public class DbHandler
    {
        private const string getFieldsQueryString =
@"SELECT 
    ISNULL(INFO.TABLE_NAME, STD.TABLENAME) as TableName,
    ISNULL(INFO.COLUMN_NAME, STD.FIELDNAME) as FieldName,
	CASE WHEN INFO.TABLE_NAME IS NULL THEN 1 ELSE 0 END as MissingFromSQL,
	CASE WHEN STD.TABLENAME IS NULL THEN 1 ELSE 0 END as MissingFromSLX,
	ISNULL(STD.DISPLAYNAME, '-') as DisplayNameSLX,
	INFO.DATA_TYPE as SqlDataType,
	INFO.CHARACTER_MAXIMUM_LENGTH as SqlMaximumLength
FROM INFORMATION_SCHEMA.COLUMNS INFO
	INNER JOIN INFORMATION_SCHEMA.TABLES TABS
		ON INFO.TABLE_NAME = TABS.TABLE_NAME AND INFO.TABLE_SCHEMA = TABS.TABLE_SCHEMA
	FULL OUTER JOIN SECTABLEDEFS STD
		ON INFO.TABLE_NAME = STD.TABLENAME AND
			INFO.COLUMN_NAME = STD.FIELDNAME
WHERE INFO.TABLE_NAME IN (SELECT DISTINCT TABLENAME FROM SECTABLEDEFS)
	AND INFO.TABLE_SCHEMA = 'sysdba'
	AND INFO.DATA_TYPE LIKE '%char%'
ORDER BY 1, 2;
";

        /// <summary>
        /// Connection String property
        /// </summary>
        public string dbConnectionString { get; set; }
        private System.Data.SqlClient.SqlConnection dbConnection;

        private string _serverName;
        private string _databaseName;
        private Microsoft.SqlServer.Management.Common.ServerConnection _serverConnection;
        
        /// <summary>
        /// Helper constructor with connection string builder
        /// </summary>
        /// <param name="serverName">The name of the Sql Server</param>
        /// <param name="databaseName">Name of the database</param>
        /// <param name="userName">Name of the user (usually sysdba)</param>
        /// <param name="password">Password</param>
        public DbHandler(string serverName, string databaseName, string userName, string password)
        {
            _serverName = serverName;
            _databaseName = databaseName;
            _serverConnection = new Microsoft.SqlServer.Management.Common.ServerConnection(
                serverName,
                userName,
                password
            );

            var connStringBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder();
            connStringBuilder.DataSource = serverName;
            connStringBuilder.InitialCatalog = databaseName;
            connStringBuilder.UserID = userName;
            connStringBuilder.Password = password;

            OpenDbConnection(connStringBuilder.ToString());
        }

        /// <summary>
        /// Opens up the Connection to the database
        /// </summary>
        /// <param name="_dbConnectionString">The Connection String</param>
        private void OpenDbConnection(string _dbConnectionString)
        {
            dbConnectionString = _dbConnectionString;
            
            OpenDbConnection();
        }
        
        /// <summary>
        /// Opens up a connection to the database using the cached connection string
        /// </summary>
        private void OpenDbConnection()
        {
            dbConnection = new System.Data.SqlClient.SqlConnection(this.dbConnectionString);

            try
            {
                dbConnection.Open();
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                throw new System.Exception("There was an error initializing database connection:\r\n" + e.Message, e);
            }
        }

        /// <summary>
        /// Closes the private database connection
        /// </summary>
        private void CloseConnection()
        {
            if (dbConnection != null && dbConnection.State != System.Data.ConnectionState.Closed)
            {
                dbConnection.Close();
            }

            dbConnection = null;
        }

        /// <summary>
        /// Creates and returns a new FieldInformationCollection instance
        /// filled with field data read from the db
        /// </summary>
        /// <returns></returns>
        public FieldInformationCollection ReadTableDataFromSLXDb()
        {
            // just update a empty collection
            return this.ReadTableDataFromSLXDb(new FieldInformationCollection());
        }

        /// <summary>
        /// Updates a FieldInformationCollection instance
        /// filling it with field data read from the db
        /// </summary>
        /// <param name="dbFields">The field collection to update</param>
        /// <returns></returns>
        public FieldInformationCollection ReadTableDataFromSLXDb(FieldInformationCollection dbFields)
        {
            OpenDbConnection();
            var cmd = dbConnection.CreateCommand();
            cmd.CommandText = DbHandler.getFieldsQueryString;
            cmd.CommandType = System.Data.CommandType.Text;

            // read the field information from the db
            var recSet = cmd.ExecuteReader();

            if (!recSet.HasRows)
                throw new Exception("No field information found in the db. Check your configuration!");

            // read sql info for each field
            while (recSet.Read())
            {
                FieldInformation f = dbFields.InitField(recSet["TableName"].ToString(), recSet["FieldName"].ToString());

                f.sqlType = recSet["SqlDataType"].ToString();
                f.sqlLength = (int) recSet["SqlMaximumLength"];
            }

            recSet.Close();
            CloseConnection();

            return dbFields;
        }

        public void SetUnicodeOnDbField(FieldInformation field, bool UnicodeEnabled)
        {
            string createScript = GetCreateScriptForIndexes(field.tableName, field.fieldName),
                dropScript = GetDropScriptForIndexes(field.tableName, field.fieldName);

            OpenDbConnection();

            // Drop the indexes on the field
            SqlCommand cmd = dbConnection.CreateCommand();
            cmd.CommandText = dropScript;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandTimeout = 3600;
            cmd.ExecuteNonQuery();

            // Actually alter the column type (using SMO)
            Server serv = new Server(_serverConnection);
            Database dbase = serv.Databases[_databaseName];
            Table table = dbase.Tables[field.tableName, "sysdba"];
            Column col = table.Columns[field.fieldName];

            col.DataType = new DataType(UnicodeEnabled ? SqlDataType.NVarChar : SqlDataType.VarChar, col.DataType.MaximumLength);

            col.Alter();

            // restores the indexes on the field
            cmd = dbConnection.CreateCommand();
            cmd.CommandText = createScript;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandTimeout = 3600;
            cmd.ExecuteNonQuery();

            CloseConnection();
        }

        public string GetCreateScriptForIndexes(string tableName, string columnName)
        {
            return GetCreateScriptForIndexes(tableName, columnName, false);
        }

        public string GetDropScriptForIndexes(string tableName, string columnName)
        {
            return GetCreateScriptForIndexes(tableName, columnName, true);
        }

        public string GetCreateScriptForIndexes(string tableName, string columnName, bool doDrop)
        {
            StringCollection sc = new StringCollection();
            ScriptingOptions so = new ScriptingOptions();
            so.ScriptDrops = doDrop;
            so.IncludeDatabaseContext = false;
            so.DriForeignKeys = true;

            StoredProcedure sp = new StoredProcedure();
            Server serv = new Server(_serverConnection);
            Database dbase = serv.Databases[_databaseName];

            StringBuilder script = new StringBuilder();
            
            Table table = dbase.Tables[tableName, "sysdba"];
            Column col = table.Columns[columnName];

            foreach(System.Data.DataRow idx in col.EnumIndexes().Rows)
            {
                string indexName = idx[0].ToString();
                sc = table.Indexes[indexName].Script(so);

                foreach (string s in sc)
                {
                    script.AppendLine(s);
                }

                script.AppendLine(";");
                script.AppendLine();
            }

            return script.ToString();
        }
    }
}
