using System;
using System.Linq;
using System.Xml.Linq;

namespace DbSafe.FileDefinition
{
    public class DatasetElement
    {
        public const string ElementName = "dataset";
        public const string AttributeName = "name";
        public const string AttributeTable = "table";
        public const string AttributeSetIdentityInsert = "setIdentityInsert";
        public const string ElementDataName = "data";

        public string Name { get; set; }

        public string Table { get; set; }

        public bool SetIdentityInsert { get; set; }

        public XElement Data { get; set; }

        public static DatasetElement Load(XElement xml)
        {
            FileDefinitionHelper.ValidateElementName(xml, ElementName);

            var result = new DatasetElement();
            result.DecodeAttributes(xml);
            result.DecodeData(xml);
            FileDefinitionHelper.ValidateAttribute(result.Name, ElementName, AttributeName);

            return result;
        }

        private void DecodeData(XElement xml)
        {
            if (xml.Elements().Count() == 0)
            {
                return;
            }

            if (xml.Elements().Count() > 1)
            {
                throw new InvalidOperationException($"Element '{DatasetElement.ElementName}' can have one child only.");
            }

            var firstNode = xml.Elements().First();
            if (firstNode.Name.LocalName != DatasetElement.ElementDataName)
            {
                throw new InvalidOperationException($"Element: '{DatasetElement.ElementName}'. Invalid child name '{firstNode.Name.LocalName}'. Expected: '{DatasetElement.ElementDataName}'.");
            }

            Data = firstNode;
        }

        private void DecodeAttributes(XElement xml)
        {
            foreach (var attribute in xml.Attributes())
            {
                switch (attribute.Name.LocalName)
                {
                    case AttributeName:
                        Name = attribute.Value;
                        break;
                    case AttributeTable:
                        Table = attribute.Value;
                        break;
                    case AttributeSetIdentityInsert:
                        bool result;
                        bool decoded = bool.TryParse(attribute.Value, out result);
                        if (!decoded)
                        {
                            throw new InvalidOperationException($"Element: '{DatasetElement.ElementName}'. Invalid attribute '{attribute.Name}'.");
                        }

                        SetIdentityInsert = result;
                        break;
                    default:
                        throw new InvalidOperationException($"Element: '{DatasetElement.ElementName}'. Invalid attribute '{attribute.Name}'.");
                }
            }
        }
    }
}
