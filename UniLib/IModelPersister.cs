using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Gianos.UniLib
{
    public interface IModelPersister
    {
        IEnumerable<XPathDocument> GetEntityFiles();

        XmlDocument OpenEntityFileForField(FieldInformation field);

        void SaveEntityFileForField(FieldInformation field, XmlDocument doc);
    }
}
