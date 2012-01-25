using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.IO;

namespace Gianos.UniLib
{
    class FSModelPersister : IModelPersister
    {
        public string modelPath { get; private set; }
        private DirectoryInfo modelDir;
		
        public FSModelPersister(string modelPath)
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

        /// <summary>
        /// Enumerates through Model dir to collect an XPathDocument
        /// for each entity xml file
        /// </summary>
        /// <returns></returns>
        public IEnumerable<XPathDocument> GetEntityFiles()
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
        /// Loads an entity XML from FS
        /// </summary>
        /// <param name="field">Field Data</param>
        /// <returns></returns>
        public XmlDocument OpenEntityFileForField(FieldInformation field)
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
        /// Saves the updated entity xml to the FS
        /// </summary>
        /// <param name="field"></param>
        /// <param name="doc"></param>
        public void SaveEntityFileForField(FieldInformation field, XmlDocument doc)
        {
            var entityModelDir = modelDir.GetDirectories("Entity Model", System.IO.SearchOption.TopDirectoryOnly)[0];
            var entityFiles = entityModelDir.GetFiles("*." + field.tableName + ".entity.xml", System.IO.SearchOption.AllDirectories);

            if (entityFiles.Length != 1)
                throw new Exception("Zero or more than 1 file found for entity " + field.tableName + "! Aborting.");

            var entityFile = entityFiles[0];

            doc.Save(entityFile.FullName);
        }

    }
}
