using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gianos.UniLib
{
    public class DbHandler
    {
        public string dbConnectionString { get; set; }
        private System.Data.SqlClient.SqlConnection dbConnection;
        

        public DbHandler(string _dbConnectionString)
        {
            OpenDbConnection(_dbConnectionString);
        }

        public DbHandler(string serverName, string databaseName, string userName, string password)
        {
            var connStringBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder();
            connStringBuilder.DataSource = serverName;
            connStringBuilder.InitialCatalog = databaseName;
            connStringBuilder.UserID = userName;
            connStringBuilder.Password = password;

            OpenDbConnection(connStringBuilder.ToString());
        }

        private void OpenDbConnection(string _dbConnectionString)
        {
            dbConnectionString = _dbConnectionString;
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
        
        
    }
}
