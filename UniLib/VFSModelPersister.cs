using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using System.Xml;
using System.IO;
using System.IO.Compression;

namespace Gianos.UniLib
{
    class VFSModelPersister : IModelPersister
    {
        /// <summary>
        /// Connection String property
        /// </summary>
        public string dbConnectionString { get; set; }
        private System.Data.SqlClient.SqlConnection dbConnection;

        private string _serverName;
        private string _databaseName;
        private Microsoft.SqlServer.Management.Common.ServerConnection _serverConnection;

        public VFSModelPersister(string serverName, string databaseName, string userName, string password)
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
        /// Opens up the Connection to the database to access VFS
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
        /// Enumerates through VFS records to get an XPathDocument
        /// for each entity xml row
        /// </summary>
        /// <returns></returns>
        public IEnumerable<XPathDocument> GetEntityFiles()
        {
            OpenDbConnection();
            var cmd = dbConnection.CreateCommand();
            cmd.CommandText =
@"SELECT ITEMDATA, ISCOMPRESSED
FROM VIRTUALFILESYSTEM
WHERE ITEMPATH LIKE '\Model\Entity Model\%' AND	ITEMNAME LIKE '%.entity.xml';";

            cmd.CommandType = System.Data.CommandType.Text;

            // read the field information from the db
            var recSet = cmd.ExecuteReader();

            if (!recSet.HasRows)
                throw new Exception("No field information found in the db. Check your configuration!");

            // read sql info for each field
            while (recSet.Read())
            {
                var ms = UnpackItemData(recSet.GetValue(0) as byte[], (recSet.GetValue(1) as string) == "T");
                string xmlString = System.Text.UTF8Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();
                yield return new XPathDocument(new System.IO.StringReader(xmlString));
            }

            recSet.Close();
            CloseConnection();

            //throw new Exception("Some error occurred retrieving field information from the db! Aborting.");

            yield break;
        }

        public XmlDocument OpenEntityFileForField(FieldInformation field)
        {
            OpenDbConnection();
            var cmd = dbConnection.CreateCommand();
            cmd.CommandText =
@"SELECT ITEMDATA, ISCOMPRESSED
FROM VIRTUALFILESYSTEM
WHERE ITEMPATH LIKE '\Model\Entity Model\%' AND	ITEMNAME LIKE '%." + field.tableName + ".entity.xml';";

            cmd.CommandType = System.Data.CommandType.Text;

            // read the field information from the db
            var recSet = cmd.ExecuteReader();

            if (!recSet.HasRows)
                throw new Exception("No field information found in the db. Check your configuration!");

            // read sql info for each field
            if (!recSet.Read()) return null;

            var ms = UnpackItemData(recSet.GetValue(0) as byte[], (recSet.GetValue(1) as string) == "T");
            string xmlString = System.Text.UTF8Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();

            XmlDocument doc = new XmlDocument();
            doc.Load(new System.IO.StringReader(xmlString));

            recSet.Close();
            CloseConnection();

            return doc;
        }

        public void SaveEntityFileForField(FieldInformation field, XmlDocument doc)
        {
            bool isCompressed = false;

            MemoryStream ms = new MemoryStream();
            doc.Save(ms);
            ms.Flush();
            ms.Position = 0;
            byte[] binaryData = PackItemData(ms, true, ref isCompressed);

            OpenDbConnection();
            var cmd = dbConnection.CreateCommand();
            cmd.CommandText =
@"UPDATE VIRTUALFILESYSTEM
SET ITEMDATA = @dataPar,
    ISCOMPRESSED = @isCompressedPar,
    MODIFYUSER = 'ADMIN',
    MODIFYDATE = GETDATE()
WHERE ITEMPATH LIKE '\Model\Entity Model\%' AND	ITEMNAME LIKE '%." + field.tableName + ".entity.xml';";

            cmd.CommandType = System.Data.CommandType.Text;

            cmd.Parameters
                .Add("@dataPar", System.Data.SqlDbType.Image)
                .Value = binaryData;

            cmd.Parameters
                .Add("@isCompressedPar", System.Data.SqlDbType.Char)
                .Value = isCompressed ? "T" : "F";

            cmd.ExecuteNonQuery();

            CloseConnection();
        }

        /// <summary>
        /// Copied and Pasted from Sage.Platform.VirtualFileSystem.VFSQuery
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="compressed"></param>
        /// <returns></returns>
        private static MemoryStream UnpackItemData(byte[] itemData, bool compressed)
        {
            if (itemData == null)
            {
                return null;
            }
            if (!compressed)
            {
                return new MemoryStream(itemData);
            }
            MemoryStream stream = new MemoryStream();
            using (MemoryStream stream2 = new MemoryStream(itemData))
            {
                using (System.IO.Compression.DeflateStream stream3 = new System.IO.Compression.DeflateStream(stream2, System.IO.Compression.CompressionMode.Decompress))
                {
                    int num;
                    byte[] buffer = new byte[0x100];
                    while ((num = stream3.Read(buffer, 0, 0x100)) > 0)
                    {
                        stream.Write(buffer, 0, num);
                    }
                }
            }
            return stream;
        }

        /// <summary>
        /// Copied and Pasted from Sage.Platform.VirtualFileSystem.VFSQuery
        /// </summary>
        /// <param name="itemData">Data to pack</param>
        /// <param name="smart">Smart compresses</param>
        /// <param name="compressed"></param>
        /// <returns></returns>
        private static byte[] PackItemData(MemoryStream itemData, bool smart, ref bool compressed)
        {
            if (itemData == null)
            {
                return null;
            }
            byte[] buffer = itemData.ToArray();
            if (smart || compressed)
            {
                byte[] buffer2;
                using (MemoryStream stream = new MemoryStream())
                {
                    using (DeflateStream stream2 = new DeflateStream(stream, CompressionMode.Compress))
                    {
                        stream2.Write(buffer, 0, buffer.Length);
                    }
                    buffer2 = stream.ToArray();
                }
                compressed = !smart || (buffer2.Length < buffer.Length);
                if (compressed)
                {
                    return buffer2;
                }
            }
            return buffer;
        }
    }
}
