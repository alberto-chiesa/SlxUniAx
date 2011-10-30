using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="_dbConnectionString">Connection String to be used</param>
        public DbHandler(string _dbConnectionString)
        {
            OpenDbConnection(_dbConnectionString);
        }

        /// <summary>
        /// Helper constructor with connection string builder
        /// </summary>
        /// <param name="serverName">The name of the Sql Server</param>
        /// <param name="databaseName">Name of the database</param>
        /// <param name="userName">Name of the user (usually sysdba)</param>
        /// <param name="password">Password</param>
        public DbHandler(string serverName, string databaseName, string userName, string password)
        {
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

        private FieldInformationCollection ReadTableDataFromSLXDb()
        {
            var dbFields = new FieldInformationCollection();

            OpenDbConnection();
            var cmd = dbConnection.CreateCommand();
            cmd.CommandText = DbHandler.getFieldsQueryString;
            cmd.CommandType = System.Data.CommandType.Text;

            var recSet = cmd.ExecuteReader();

            if (!recSet.HasRows)
                throw new Exception("No field information found in the db. Check your configuration!");

            while (recSet.Read())
            {
            }

            return dbFields;
        }
    }
}
