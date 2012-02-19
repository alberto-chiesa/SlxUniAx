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
    /// <summary>
    /// VFS Model Persister. Implements methods for
    /// reading and updating entity metadata coming
    /// from the VFS table.
    /// </summary>
    class VFSModelPersister : IModelPersister
    {
        private const string SelectEntityQueryText =
@"SELECT ITEMDATA, ISCOMPRESSED
FROM VIRTUALFILESYSTEM
WHERE ITEMPATH LIKE '\Model\Entity Model\%' AND	ITEMNAME LIKE '%.entity.xml';";

        private const string SelectFileQueryText =
@"SELECT ITEMDATA, ISCOMPRESSED
FROM VIRTUALFILESYSTEM
WHERE ITEMPATH LIKE '\Model\Entity Model\%' AND	ITEMNAME LIKE '%.{0}.entity.xml';";

        private const string UpdateFileQueryText =
@"UPDATE VIRTUALFILESYSTEM
SET ITEMDATA = @dataPar,
    ISCOMPRESSED = @isCompressedPar,
    MODIFYUSER = 'ADMIN',
    MODIFYDATE = GETDATE()
WHERE ITEMPATH LIKE '\Model\Entity Model\%' AND	ITEMNAME LIKE '%.{0}.entity.xml';";

        /// <summary>
        /// Connection String property
        /// </summary>
        private string dbConnectionString { get; set; }
        private System.Data.SqlClient.SqlConnection dbConnection;

        public VFSModelPersister(string serverName, string databaseName, string userName, string password)
        {
            var connStringBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder()
                {
                    DataSource = serverName,
                    InitialCatalog = databaseName,
                    UserID = userName,
                    Password = password
                };

            dbConnectionString = connStringBuilder.ToString();
            OpenDbConnection();
            CloseConnection();
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
            var cmd = CreateSqlCommand(VFSModelPersister.SelectEntityQueryText);

            // read the field information from the db
            var recSet = cmd.ExecuteReader();

            if (!recSet.HasRows)
                throw new Exception("No field information found in the db. Check your configuration!");

            // read sql info for each field
            while (recSet.Read())
            {
                var sr = GetStringReaderFromVFSRecordData(recSet.GetValue(0) as byte[], (recSet.GetValue(1) as string) == "T");
                yield return new XPathDocument(sr);
            }

            recSet.Close();
            CloseConnection();

            yield break;
        }

        private System.Data.SqlClient.SqlCommand CreateSqlCommand(string cmdText)
        {
            var cmd = dbConnection.CreateCommand();
            cmd.CommandText = cmdText;
            cmd.CommandType = System.Data.CommandType.Text;
            return cmd;
        }

        public XmlDocument OpenEntityFileForField(FieldInformation field)
        {
            OpenDbConnection();
            var cmd = CreateSqlCommand(String.Format(SelectFileQueryText, field.tableName));

            // read the field information from the db
            var recSet = cmd.ExecuteReader();

            if (!recSet.HasRows)
                throw new Exception("No field information found in the db. Check your configuration!");

            // read sql info for each field
            if (!recSet.Read()) return null;
            
            XmlDocument doc = BuildXmlDocumentFromRAWEntityFile(recSet.GetValue(0) as byte[], (recSet.GetValue(1) as string) == "T");

            recSet.Close();
            CloseConnection();

            return doc;
        }

        private static XmlDocument BuildXmlDocumentFromRAWEntityFile(byte[] data, bool mustCompress)
        {
            var sr = GetStringReaderFromVFSRecordData(data, mustCompress);

            XmlDocument doc = new XmlDocument();
            doc.Load(sr);
            
            return doc;
        }

        private static System.IO.StringReader GetStringReaderFromVFSRecordData(byte[] data, bool mustCompress)
        {
            var ms = UnpackItemData(data, mustCompress);
            string xmlString = System.Text.UTF8Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();

            return new System.IO.StringReader(xmlString);
        }

        public void SaveEntityFileForField(FieldInformation field, XmlDocument doc)
        {
            bool isCompressed = false;

            byte[] binaryData = BuildEntityFileBinaryData(doc, ref isCompressed);

            OpenDbConnection();
            var cmd = CreateSqlCommand(String.Format(UpdateFileQueryText, field.tableName));

            cmd.Parameters.Add("@dataPar", System.Data.SqlDbType.Image)
                .Value = binaryData;

            cmd.Parameters.Add("@isCompressedPar", System.Data.SqlDbType.Char)
                .Value = isCompressed ? "T" : "F";

            cmd.ExecuteNonQuery();

            CloseConnection();
        }

        /// <summary>
        /// Encodes and compresses a XML document into a byte[]
        /// </summary>
        /// <param name="doc">XML document to encode</param>
        /// <param name="isCompressed">If true, the returned byte[] is deflate-compressed</param>
        /// <returns>Byte[] ready for insert in VFS table</returns>
        private static byte[] BuildEntityFileBinaryData(XmlDocument doc, ref bool isCompressed)
        {
            MemoryStream ms = new MemoryStream();
            doc.Save(ms);
            ms.Flush();
            ms.Position = 0;

            return PackItemData(ms, true, ref isCompressed);
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
