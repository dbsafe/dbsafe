using DbSafe.FileDefinition;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;

namespace DbSafeTests.FileDefinition
{
    [TestClass]
    public class DatasetElementTest
    {
        [TestMethod]
        public void Load_Given_input_with_invalid_element_An_exception_is_raised()
        {
            var xml = @"<an-invalid-element/>";
            var expectedMessage = "Invalid element name: 'an-invalid-element'. Expected: 'dataset'.";

            try
            {
                ExecuteLoad(xml);
                Assert.Fail("The expected exception was not raised.");
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(expectedMessage, ex.Message);
            }
        }

        [TestMethod]
        public void Load_Given_input_Property_name_is_correct()
        {
            var xml = @"<dataset name=""ds1"" />";

            var actual = ExecuteLoad(xml);
            Assert.AreEqual("ds1", actual.Name);
        }

        [TestMethod]
        public void Load_Given_input_Property_data_is_correct()
        {
            var xml = @"<dataset name=""ds1"" ><data attr1=""v1"" /></dataset>";
            var expectedData = @"<data attr1=""v1"" />";

            var actual = ExecuteLoad(xml);

            Assert.IsNotNull(actual.Data);
            AssertXml(expectedData, actual.Data.ToString());
        }

        [TestMethod]
        public void Load_Given_input_with_missing_name_attribute_An_exception_is_raised()
        {
            string xml = @"<dataset/>";
            string expectedMessage = "Element: 'dataset'. Attribute 'name' is required.";

            try
            {
                ExecuteLoad(xml);
                Assert.Fail("The expected exception was not raised.");
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(expectedMessage, ex.Message);
            }
        }

        [TestMethod]
        public void Load_Given_input_with_an_invalid_attribute_An_exception_is_raised()
        {
            string xml = @"<dataset name=""ds1"" an-invalid-attribute=""s1"" />";
            string expectedMessage = "Element: 'dataset'. Invalid attribute 'an-invalid-attribute'.";

            try
            {
                ExecuteLoad(xml);
                Assert.Fail("The expected exception was not raised.");
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(expectedMessage, ex.Message);
            }
        }

        [TestMethod]
        public void Load_Given_an_input_with_more_than_one_child_An_exception_is_raised()
        {
            string xml = @"
<dataset name=""ds1"" >
    <data attr1=""v1"" />
    <data attr1=""v2"" />
</dataset>";
            string expectedMessage = "Element 'dataset' can have one child only.";

            try
            {
                ExecuteLoad(xml);
                Assert.Fail("The expected exception was not raised.");
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(expectedMessage, ex.Message);
            }
        }

        [TestMethod]
        public void Load_Given_input_with_an_invalid_child_An_exception_is_raised()
        {
            string xml = @"
<dataset name=""ds1"" >
    <an-invalid-element attr1=""v1"" />
</dataset>";
            string expectedMessage = "Element: 'dataset'. Invalid child name 'an-invalid-element'. Expected: 'data'.";

            try
            {
                ExecuteLoad(xml);
                Assert.Fail("The expected exception was not raised.");
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(expectedMessage, ex.Message);
            }
        }

        private DatasetElement ExecuteLoad(string xmlText)
        {
            XElement xml = XElement.Parse(xmlText);
            return DatasetElement.Load(xml);
        }

        private void AssertXml(string expected, string actual)
        {
            var expectedXml = XElement.Parse(expected);
            var actualXml = XElement.Parse(actual);

            Assert.AreEqual(expectedXml.ToString(), actualXml.ToString());
        }
    }
}
