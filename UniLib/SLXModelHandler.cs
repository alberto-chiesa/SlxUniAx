using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.XPath;
using System.Xml;

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

        public SLXModelHandler(string modelPath)
        {
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

        public FieldInformationCollection FindEntityModels()
        {
            return FindEntityModels(new FieldInformationCollection());
        }

        public FieldInformationCollection FindEntityModels(FieldInformationCollection dbFields)
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
                var entityModelDir = modelDir.GetDirectories("Entity Model", System.IO.SearchOption.TopDirectoryOnly)[0];
                var entityFiles = entityModelDir.GetFiles("*.entity.xml", System.IO.SearchOption.AllDirectories);

                foreach (var entityFile in entityFiles)
                {
                    var fileReader = entityFile.OpenText();
                    //string  = fileReader.ReadToEnd();
                    var entityXml = new XPathDocument(fileReader);
                    fileReader.Close();

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
                            f.slxLength = property.GetAttribute("maxLength", String.Empty).ToString();
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

        public void SetUnicodeOnSlxField(FieldInformation field, bool UnicodeEnabled)
        {
            try
            {
                var entityModelDir = modelDir.GetDirectories("Entity Model", System.IO.SearchOption.TopDirectoryOnly)[0];
                var entityFiles = entityModelDir.GetFiles("*." + field.tableName + ".entity.xml", System.IO.SearchOption.AllDirectories);

                if (entityFiles.Length != 1)
                    throw new Exception("Zero or more than 1 file found for entity " + field.tableName + "! Aborting.");

                var entityFile = entityFiles[0];

                XmlDocument doc = new XmlDocument();
                doc.Load(entityFile.FullName);

                var nav = doc.CreateNavigator();

                var property = nav.SelectSingleNode(String.Format(@"//property[@columnName='{0}']", field.fieldName));

                var slxLength = property.GetAttribute("maxLength", String.Empty).ToString();

                var newTypeXML = (UnicodeEnabled ? unicodeTextPropertyXMLModel : textPropertyXMLModel);

                newTypeXML = newTypeXML.Replace("******", slxLength);

                property.SelectSingleNode("SystemDataType").OuterXml = newTypeXML;

                doc.Save(entityFile.FullName);
            }
            catch (Exception e)
            {
                throw e;// new Exception("Error reading entity model files!");
            }
        }

        public void ApplyActionsToModel(FieldAction[] actions)
        {
            foreach (var action in actions)
            {
                this.SetUnicodeOnSlxField(action.FieldInfo, action.NewState == FieldState.Unicode);
            }
        }
    }
}
