using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.XPath;
using System.Xml;
using System.Data.OleDb;

namespace Gianos.UniLib
{
    public class SLXModelHandler
    {
        const string textPropertyXMLModel = @"<SystemDataType guid=""ccc0f01d-7ba5-408e-8526-a3f942354b3a"">
<TextDataType>
<Length>******</Length>
</TextDataType>
</SystemDataType>";

        const string unicodeTextPropertyXMLModel = @"<SystemDataType guid=""76c537a8-8b08-4b35-84cf-fa95c6c133b0"">
<UnicodeTextDataType>
<Length>******</Length>
</UnicodeTextDataType>
</SystemDataType>";

        public string modelPath { get; private set; }
        private DirectoryInfo modelDir;
		
		private bool usingVFS {get; set; }

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
        public SLXModelHandler(string serverName, string databaseName, string userName, string password)
        {
			usingVFS = true;
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

        public SLXModelHandler(string modelPath)
        {
			usingVFS = false;
			
            this.modelPath = modelPath;
            this.modelDir = new System.IO.DirectoryInfo(modelPath);
            if (!modelDir.Exists)
                throw new Exception(String.Format("The model path {0} was not found!", modelPath));

            try
            {
                var entityModelFolder = modelDir.GetDirectories("Entity Model", System.IO.SearchOption.TopDirectoryOnly)[0];
            }
            catch (Exception e)
            {
                throw new Exception("The Entity Model folder was not found inside the provided model folder!", e);
            }
        }

        /// <summary>
        /// Enumerates through Model dir to collect an XPathDocument
        /// for each entity xml file
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.IEnumerable<XPathDocument> GetModelFilesFromFS()
        {
            var entityModelDir = modelDir.GetDirectories("Entity Model", System.IO.SearchOption.TopDirectoryOnly)[0];
            var entityFiles = entityModelDir.GetFiles("*.entity.xml", System.IO.SearchOption.AllDirectories);

            foreach (var entityFile in entityFiles)
            {
                var fileReader = entityFile.OpenText();
                var entityXml = new XPathDocument(fileReader);
                fileReader.Close();

                yield return entityXml;
            }

            yield break;
        }

        /// <summary>
        /// Enumerates through VFS records to get an XPathDocument
        /// for each entity xml row
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.IEnumerable<XPathDocument> GetModelFilesFromVFS()
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
                var ms = Utils.UnpackItemData(recSet.GetValue(0) as byte[], (recSet.GetValue(1) as string) == "T");
                string xmlString = System.Text.UTF8Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();
                yield return new XPathDocument(new System.IO.StringReader(xmlString));
            }

            recSet.Close();
            CloseConnection();

            //throw new Exception("Some error occurred retrieving field information from the db! Aborting.");

            yield break;
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
        /// Fills a new FieldInformationManager instance
        /// with entity metadata coming from the Model
        /// </summary>
        /// <returns></returns>
        public FieldInformationManager FindEntityModels()
        {
            return FindEntityModels(new FieldInformationManager());
        }


        /// <summary>
        /// Fills the provided FieldInformationManager instance
        /// with entity metadata coming from the Model
        /// </summary>
        /// <param name="dbFields"></param>
        /// <returns></returns>
        public FieldInformationManager FindEntityModels(FieldInformationManager dbFields)
        {
            /* Example XML property snippet:
             * <property xsi:type="OrmFieldProperty" id="d6e61b3a-4663-4d89-a605-beb0fc07cae2" lastModifiedUtc="2009-04-20T02:51:53.3856462Z" name="LastName" audited="false" columnName="LASTNAME" maxLength="50" precision="0" scale="0" ordinal="5" isReadOnly="false" isDynamic="false">
             *   <ExtendedPropertiesCollection>
             *     <extendee type="Sage.Platform.Orm.Entities.OrmFieldProperty" />
             *   </ExtendedPropertiesCollection>
             *   <SystemDataType guid="ccc0f01d-7ba5-408e-8526-a3f942354b3a">
             *     <TextDataType>
             *       <Length>50</Length>
             *     </TextDataType>
             *   </SystemDataType>
             * </property>
             * <property xsi:type="OrmFieldProperty" id="37a5bc92-84cc-4e43-ab62-6c9c6208bb84" lastModifiedUtc="2011-04-13T14:56:08.9030225Z" name="CountryLocal" audited="false" columnName="COUNTRYLOCAL" maxLength="64" precision="0" scale="0" ordinal="8" isReadOnly="false" isDynamic="false">
             *   <ExtendedPropertiesCollection>
             * 	<extendee type="Sage.Platform.Orm.Entities.OrmFieldProperty" />
             *   </ExtendedPropertiesCollection>
             *   <SystemDataType guid="76c537a8-8b08-4b35-84cf-fa95c6c133b0">
             * 	<UnicodeTextDataType>
             * 	  <Length>64</Length>
             * 	</UnicodeTextDataType>
             *   </SystemDataType>
             * </property>
             */
            try
            {
                var entityXMLs = (this.usingVFS ? GetModelFilesFromVFS() : GetModelFilesFromFS());
                
                foreach (XPathDocument entityXml in entityXMLs)
                {
                    var nav = entityXml.CreateNavigator();

                    var tableName = nav.SelectSingleNode("//entity").GetAttribute("tableName", String.Empty);

                    var properties = nav.Select("//property");
                    foreach (XPathNavigator property in properties)
                    {
                        string slxType = String.Empty;
                        if (property.Select("SystemDataType/TextDataType").Count > 0)
                        {
                            slxType = "TextDataType";
                        }
                        else if (property.Select("SystemDataType/UnicodeTextDataType").Count > 0)
                        {
                            slxType = "UnicodeTextDataType";
                        }

                        if (!String.IsNullOrEmpty(slxType))
                        {
                            var propertyName = property.GetAttribute("columnName", String.Empty).ToString();
                            FieldInformation f = dbFields.InitField(tableName.ToUpper(), propertyName.ToUpper());
                            f.slxType = slxType;
                            var lengthStr = property.GetAttribute("maxLength", String.Empty).ToString();
                            int len;

                            f.slxLength = Int32.TryParse(lengthStr, out len) ? len : 0;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Error reading entity model files!");
            }

            return dbFields;
        }

        /// <summary>
        /// Main update method: sets the field type (and size) based on the provided options
        /// </summary>
        /// <param name="field">Provides entity and property names</param>
        /// <param name="UnicodeEnabled">True if must enable Unicode, false otherwise</param>
        /// <param name="newSize">New size</param>
        public void SetUnicodeOnSlxField(FieldInformation field, bool UnicodeEnabled, int newSize)
        {
            try
            {
                XmlDocument doc = this.usingVFS ? OpenXmlDocumentFromVFS(field) : OpenXmlDocumentFromFS(field);

                var property = doc.SelectSingleNode(String.Format(@"//property[@columnName='{0}']", field.fieldName));
                
				property.Attributes["lastModifiedUtc"].Value = DateTime.UtcNow.ToString("o");

                var currentSlxLength = property.Attributes["maxLength"].Value;

                if (newSize > 0)
                {
                    property.Attributes["maxLength"].Value = newSize.ToString();
                }

                // new xml snippet: use provided models and replace ***** with length
                var newTypeXML = (UnicodeEnabled ? unicodeTextPropertyXMLModel : textPropertyXMLModel)
                    .Replace("******", (newSize > 0 ? newSize.ToString() : currentSlxLength));

                string xpathSelector = String.Format(@"//property[@columnName='{0}']/SystemDataType", field.fieldName); 
                
                doc.CreateNavigator().SelectSingleNode(xpathSelector).OuterXml = newTypeXML;

                if (this.usingVFS)
                    SaveXmlDocumentToVFS(field, doc);
                else
                    SaveXmlDocumentToFS(field, doc);
            }
            catch (Exception e)
            {
                throw e;// new Exception("Error reading entity model files!");
            }
        }

        private void SaveXmlDocumentToVFS(FieldInformation field, XmlDocument doc)
        {
            throw new NotImplementedException();
        }

        private XmlDocument OpenXmlDocumentFromVFS(FieldInformation field)
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

            var ms = Utils.UnpackItemData(recSet.GetValue(0) as byte[], (recSet.GetValue(1) as string) == "T");
            string xmlString = System.Text.UTF8Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();

            XmlDocument doc = new XmlDocument();
            doc.Load(new System.IO.StringReader(xmlString));

            recSet.Close();
            CloseConnection();
            
            return doc;
        }

        /// <summary>
        /// Saves the updated entity xml to the FS
        /// </summary>
        /// <param name="field"></param>
        /// <param name="doc"></param>
        private void SaveXmlDocumentToFS(FieldInformation field, XmlDocument doc)
        {
            var entityModelDir = modelDir.GetDirectories("Entity Model", System.IO.SearchOption.TopDirectoryOnly)[0];
            var entityFiles = entityModelDir.GetFiles("*." + field.tableName + ".entity.xml", System.IO.SearchOption.AllDirectories);

            if (entityFiles.Length != 1)
                throw new Exception("Zero or more than 1 file found for entity " + field.tableName + "! Aborting.");

            var entityFile = entityFiles[0];

            doc.Save(entityFile.FullName);
        }

        /// <summary>
        /// Loads an entity XML from FS
        /// </summary>
        /// <param name="field">Field Data</param>
        /// <returns></returns>
        private XmlDocument OpenXmlDocumentFromFS(FieldInformation field)
        {
            var entityModelDir = modelDir.GetDirectories("Entity Model", System.IO.SearchOption.TopDirectoryOnly)[0];
            var entityFiles = entityModelDir.GetFiles("*." + field.tableName + ".entity.xml", System.IO.SearchOption.AllDirectories);

            if (entityFiles.Length != 1)
                throw new Exception("Zero or more than 1 file found for entity " + field.tableName + "! Aborting.");

            var entityFile = entityFiles[0];

            var tr = new System.IO.FileStream(entityFile.FullName, FileMode.Open);

            XmlDocument doc = new XmlDocument();
            doc.Load(entityFile.FullName);

            return doc;
        }

        /// <summary>
        /// Iterates over a list of actions and applies them to the model
        /// </summary>
        /// <param name="actions"></param>
        public void ApplyActionsToModel(FieldAction[] actions)
        {
            foreach (var action in actions)
            {
                this.SetUnicodeOnSlxField(action.FieldInfo, action.NewState == FieldState.Unicode, action.NewSize);
            }
        }
    }
}
