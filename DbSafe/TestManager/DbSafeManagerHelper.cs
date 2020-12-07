using DbSafe.FileDefinition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnitTesting = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbSafe
{
    public static class DbSafeManagerHelper
    {
        public static void CompareDatasets(DatasetElement expected, DatasetElement actual, string[] keys, bool sorted, bool compareTableName = true)
        {
            if (compareTableName)
            {
                UnitTesting.Assert.AreEqual(expected.Table, actual.Table, "Table names are different.");
            }

            if (expected.Data == null)
            {
                UnitTesting.Assert.IsNull(actual.Data, "Expected Data is null but actual Data is not null");
                return;
            }
            else
            {
                UnitTesting.Assert.IsNotNull(actual.Data, "Expected Data is not null but actual Data is null");
            }
            
            UnitTesting.Assert.AreEqual(expected.Data.Elements().Count(), actual.Data.Elements().Count(), "The number of rows are different.");
            if (sorted)
            {
                CompareSortedElements(expected.Data, actual.Data);
            }
            else
            {
                CompareUnsortedElements(expected.Data, actual.Data, keys);
            }
        }

        public static void CompareUnsortedElements(XElement expected, XElement actual, string[] keys)
        {
            foreach (var expectedElement in expected.Elements())
            {
                List<KeyValuePair<string, string>> keysAndValues = new List<KeyValuePair<string, string>>();
                foreach (var key in keys)
                {
                    string value = string.Empty;
                    var attribute = expectedElement.Attribute(key);

                    if (attribute != null)
                    {
                        value = attribute.Value;
                    }

                    var item = new KeyValuePair<string, string>(key, value);
                    keysAndValues.Add(item);
                }

                XElement actualElement = FindElementByKeyAndValues(keysAndValues, actual);
                CompareElements(expectedElement, actualElement);
            }
        }

        public static void CompareSortedElements(XElement expected, XElement actual)
        {
            var expectedArray = expected.Elements()
                .ToArray();

            var actualArray = actual.Elements()
                .ToArray();

            for (int i = 0; i < expectedArray.Length; i++)
            {
                var expectedElement = expectedArray[i];
                var actualElement = actualArray[i];
                CompareElements(expectedElement, actualElement);
            }
        }

        public static void CompareElements(XElement expected, XElement actual)
        {
            var summary = $@"
Expected:
{expected}
Actual:
{actual}
";
            UnitTesting.Assert.AreEqual(expected.Attributes().Count(), actual.Attributes().Count(), $"The number of attributes are not the same. {summary}");

            foreach (var expectedAttribute in expected.Attributes())
            {
                var actualAttribute = actual.Attribute(expectedAttribute.Name);
                UnitTesting.Assert.IsNotNull(actualAttribute, $"Attribute '{expectedAttribute.Name}' not found in actual data. {summary}");
                UnitTesting.Assert.AreEqual(expectedAttribute.Value, actualAttribute.Value, $"Values for field '{expectedAttribute.Name}' are different. {summary}");
            }
        }

        public static XElement FindElementByKeyAndValues(List<KeyValuePair<string, string>> keysAndValues, XElement xml)
        {
            // validate that all the key attributes are found.
            foreach (var element in xml.Elements())
            {
                foreach (var keyAndValue in keysAndValues)
                {
                    var attribute = element.Attribute(keyAndValue.Key);
                    if (attribute == null)
                    {
                        throw new InvalidOperationException($"Key attribute '{keyAndValue.Key}' not found. Element: {element}");
                    }
                }
            }

            var query = xml.Elements()
                .Where(a => a.Attribute(keysAndValues[0].Key).Value == keysAndValues[0].Value)
                .Select(a => a);

            for (int i = 1; i < keysAndValues.Count; i++)
            {
                int local = i;
                query = query
                    .Where(a => a.Attribute(keysAndValues[local].Key).Value == keysAndValues[local].Value)
                    .Select(a => a);
            }

            var result = query.ToArray();
            UnitTesting.Assert.IsFalse(result.Length == 0, "Row not found. {0}", KeysAndValuesToString(keysAndValues));
            UnitTesting.Assert.IsFalse(result.Length > 1, "More than one row found. {0}", KeysAndValuesToString(keysAndValues));

            return result[0];
        }

        public static string KeysAndValuesToString(List<KeyValuePair<string, string>> keysAndValues)
        {
            var list = keysAndValues.Select(a => string.Format("Key: {0}, Value: {1}", a.Key, a.Value));
            return string.Join(";", list);
        }

        public static XElement FindChild(XElement xml, string parentName, string childName)
        {
            XElement parentElement = xml.Elements()
                .FirstOrDefault(a => a.Name.LocalName == parentName);

            if (parentElement != null)
            {
                foreach (var child in parentElement.Elements())
                {
                    var attribute = child.Attribute("name");
                    if (attribute == null)
                    {
                        continue;
                    }

                    if (attribute.Value == childName)
                    {
                        return child;
                    }
                }
            }

            return null;
        }
    }
}
