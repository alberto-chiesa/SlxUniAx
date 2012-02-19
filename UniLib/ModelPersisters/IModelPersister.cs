using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Gianos.UniLib
{
    /// <summary>
    /// Defines methods used to read and update entity
    /// metadata coming from the web model.
    /// Abstracts out VFS and FS implementation
    /// </summary>
    public interface IModelPersister
    {
        /// <summary>
        /// Get an IEnumerable&lt;XPathDocuments&gt;, one for each entity.
        /// </summary>
        /// <returns></returns>
        IEnumerable<XPathDocument> GetEntityFiles();

        /// <summary>
        /// Get a entity model file as an updatable XmlDocument.
        /// </summary>
        /// <param name="field">Field used to identity the table.</param>
        /// <returns>XmlDocument of the pertaining entity.</returns>
        XmlDocument OpenEntityFile(string tableName);

        /// <summary>
        /// Saves the passed XmlDocument to the model, for the entity
        /// identified by the passed field.
        /// </summary>
        /// <param name="field">Field defining the table</param>
        /// <param name="doc">Xml Document to be persisted</param>
        void SaveEntityFile(string tableName, XmlDocument doc);
    }
}
