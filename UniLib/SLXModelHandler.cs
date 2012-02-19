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
        const string textPropertyXMLModel =
@"<SystemDataType guid=""ccc0f01d-7ba5-408e-8526-a3f942354b3a"">
<TextDataType>
<Length>******</Length>
</TextDataType>
</SystemDataType>";

        const string unicodeTextPropertyXMLModel = 
@"<SystemDataType guid=""76c537a8-8b08-4b35-84cf-fa95c6c133b0"">
<UnicodeTextDataType>
<Length>******</Length>
</UnicodeTextDataType>
</SystemDataType>";

        private IModelPersister ModelPersister { get; set; }
        
        /// <summary>
        /// Helper constructor with connection string builder
        /// </summary>
        /// <param name="serverName">The name of the Sql Server</param>
        /// <param name="databaseName">Name of the database</param>
        /// <param name="userName">Name of the user (usually sysdba)</param>
        /// <param name="password">Password</param>
        public SLXModelHandler(string serverName, string databaseName, string userName, string password)
        {
            this.ModelPersister = new VFSModelPersister(serverName, databaseName, userName, password);
        }

		public SLXModelHandler(string modelPath)
        {
            this.ModelPersister = new FSModelPersister(modelPath);
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
                foreach (XPathDocument entityXml in ModelPersister.GetEntityFiles())
                {
                    ReadFieldInformationsForEntity(dbFields, entityXml.CreateNavigator());
                }
            }
            catch (Exception)
            {
                throw new Exception("Error reading entity model files!");
            }

            return dbFields;
        }

        private static void ReadFieldInformationsForEntity(FieldInformationManager dbFields, XPathNavigator nav)
        {
            var tableName = nav.SelectSingleNode("//entity").GetAttribute("tableName", String.Empty);

            foreach (XPathNavigator property in nav.Select("//property"))
            {
                string slxType = GetSlxTypeOfXmlProperty(property);

                if (!String.IsNullOrEmpty(slxType))
                {
                    var propertyName = property.GetAttribute("columnName", String.Empty).ToString();
                    var field = dbFields.InitField(tableName.ToUpper(), propertyName.ToUpper());

                    field.slxType = slxType;
                    field.slxLength = GetFieldLengthFromPropertyXml(property);
                }
            }
        }

        private static string GetSlxTypeOfXmlProperty(XPathNavigator property)
        {
            if (property.Select("SystemDataType/TextDataType").Count > 0)
                return "TextDataType";
            else if (property.Select("SystemDataType/UnicodeTextDataType").Count > 0)
                return "UnicodeTextDataType";

            return String.Empty;
        }

        private static Int32 GetFieldLengthFromPropertyXml(XPathNavigator property)
        {
            var lengthStr = property.GetAttribute("maxLength", String.Empty);
            int len;

            return Int32.TryParse(lengthStr, out len) ? len : 0;
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
                XmlDocument doc = ModelPersister.OpenEntityFile(field.tableName);

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

                ModelPersister.SaveEntityFile(field.tableName, doc);
            }
            catch (Exception)
            {
                throw;// new Exception("Error reading entity model files!");
            }
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
