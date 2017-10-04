using System;
using System.Xml.Linq;

namespace DbSafe.FileDefinition
{
    internal static class FileDefinitionHelper
    {
        public const string ElementNameDatasets = "datasets";
        public const string ElementNameSripts = "scripts";

        public static void ValidateAttribute(string value, string elementName, string attributeName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"Element: '{elementName}'. Attribute '{attributeName}' is required.");
            }
        }

        public static void ValidateElementName(XElement xml, string elementName)
        {
            if (xml.Name != elementName)
            {
                throw new InvalidOperationException($"Invalid element name: '{xml.Name}'. Expected: '{elementName}'.");
            }
        }
    }
}
