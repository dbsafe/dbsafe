using System;
using System.IO;
using System.Xml.Linq;

namespace DbSafe.FileDefinition
{
    public class ScriptElement
    {
        public const string ElementName = "script";
        public const string AttributeName = "name";
        public const string AttributeSource = "source";

        public ScriptElement()
        {
            Source = ScriptType.Command;
        }

        public string Name { get; set; }

        public ScriptType Source { get; set; }

        public string Value { get; set; }

        public static ScriptElement Load(XElement xml, string filename)
        {
            FileDefinitionHelper.ValidateElementName(xml, ElementName);

            var result = new ScriptElement();
            result.DecodeAttributes(xml);
            result.DecodeValue(xml, filename);
            FileDefinitionHelper.ValidateAttribute(result.Name, ElementName, AttributeName);

            return result;
        }

        private void DecodeValue(XElement xml, string filename)
        {
            switch (Source)
            {
                case ScriptType.Command:
                    Value = xml.Value;
                    break;

                case ScriptType.File:
                    if (File.Exists(xml.Value))
                    {
                        Value = xml.Value;
                        return;
                    }

                    string path = Path.GetDirectoryName(filename);
                    path = Path.Combine(path, xml.Value);
                    if (!File.Exists(path))
                    {
                        throw new FileNotFoundException(path);
                    }

                    Value = File.ReadAllText(path);
                    break;

                default:
                    throw new InvalidOperationException($"Invalid Source '{Source}'");
            }
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
                    case AttributeSource:
                        try
                        {
                            Source = (ScriptType)Enum.Parse(typeof(ScriptType), attribute.Value);
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException($"Element: '{ElementName}', Attribute: '{attribute.Name}'. The value '{attribute.Value}' is invalid.", ex);
                        }

                        break;
                    default:
                        throw new InvalidOperationException($"Element: '{ElementName}'. Invalid attribute '{attribute.Name}'.");
                }
            }
        }

        public enum ScriptType
        {
            Command,
            File
        }
    }
}
