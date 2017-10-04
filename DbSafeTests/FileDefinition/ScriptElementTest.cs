using DbSafe.FileDefinition;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Xml.Linq;

namespace DbSafeTests.FileDefinition
{
    [TestClass]
    public class ScriptElementTest
    {
        [TestMethod]
        public void Load_Given_input_with_invalid_element_An_exception_is_raised()
        {
            var xml = @"<an-invalid-element/>";
            var expectedMessage = "Invalid element name: 'an-invalid-element'. Expected: 'script'.";

            try
            {
                ExecuteLoad(xml, "is-not-a-file");
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
            var xml = @"<script name=""ds1"" />";

            var actual = ExecuteLoad(xml, "is-not-a-file");
            Assert.AreEqual("ds1", actual.Name);
        }

        [TestMethod]
        public void Load_Given_input_Property_source_is_correct()
        {
            var xml = @"<script name=""ds1"" source=""Command"" />";

            var actual = ExecuteLoad(xml, "is-not-a-file");
            Assert.AreEqual(ScriptElement.ScriptType.Command, actual.Source);
        }

        [TestMethod]
        public void Load_Given_input_with_an_invalid_source_An_exception_is_raised()
        {
            var xml = @"<script name=""ds1"" source=""bad-source"" />";
            var expectedMessage = "Element: 'script', Attribute: 'source'. The value 'bad-source' is invalid.";

            try
            {
                ExecuteLoad(xml, "is-not-a-file");
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
            var xml = @"<script name=""ds1"" some-attribute="""" />";
            var expectedMessage = "Element: 'script'. Invalid attribute 'some-attribute'.";

            try
            {
                ExecuteLoad(xml, "is-not-a-file");
                Assert.Fail("The expected exception was not raised.");
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual(expectedMessage, ex.Message);
            }
        }

        [TestMethod]
        public void Load_Given_input_Property_value_is_correct()
        {
            var xml = @"<script name=""ds1"" source=""Command"" >the sql command</script>";

            var actual = ExecuteLoad(xml, "is-not-a-file");
            Assert.AreEqual("the sql command", actual.Value);
        }

        [TestMethod]
        [DeploymentItem(@"FileDefinition\sql-script.sql")]
        public void Load_Given_input_and_script_in_file_Property_value_is_correct()
        {
            var xml = @"<script name=""ds1"" source=""File"" >sql-script.sql</script>";
            var filename = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            filename = Path.Combine(filename, "FileDefinition", "script-element-test.xml");

            var actual = ExecuteLoad(xml, filename);
            Assert.AreEqual("a large sql command", actual.Value);
        }

        private ScriptElement ExecuteLoad(string xmlText, string filename)
        {
            XElement xml = XElement.Parse(xmlText);
            return ScriptElement.Load(xml, filename);
        }
    }
}
