using DbSafe;
using DbSafe.FileDefinition;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace DbSafeTests.TestManager
{
    [TestClass]
    public class DbSafeManagerHelperTest
    {
        [TestMethod]
        public void CompareDatasets_Given_dataset_with_different_table_names_Method_fails()
        {
            var elementA = XElement.Parse(@"<dataset name=""A"" table=""tableA""></dataset>");
            var elementB = XElement.Parse(@"<dataset name=""B"" table=""tableB""></dataset>");

            var datasetElementA = DatasetElement.Load(elementA);
            var datasetElementB = DatasetElement.Load(elementB);

            var compareTableName = true;
            try
            {
                DbSafeManagerHelper.CompareDatasets(datasetElementA, datasetElementB, new string[] { "key" }, false, compareTableName);
            }
            catch (AssertFailedException ex)
            {
                StringAssert.Contains(ex.Message, "Table names are different");
                return;
            }

            Assert.Fail("An exception was not thrown");
        }

        [TestMethod]
        public void CompareDatasets_Given_null_expected_data_and_actual_data_Method_succeed()
        {
            var elementA = XElement.Parse(@"<dataset name=""A"" table=""tableA""></dataset>");
            var elementB = XElement.Parse(@"<dataset name=""B"" table=""tableA""></dataset>");

            var datasetElementA = DatasetElement.Load(elementA);
            var datasetElementB = DatasetElement.Load(elementB);

            DbSafeManagerHelper.CompareDatasets(datasetElementA, datasetElementB, new string[] { "key" }, false);
        }

        [TestMethod]
        public void CompareDatasets_Given_null_expected_data_and_not_null_actual_data_Method_fails()
        {
            var elementA = XElement.Parse(@"<dataset name=""A"" table=""tableA""></dataset>");
            var elementB = XElement.Parse(@"<dataset name=""B"" table=""tableA""><data/></dataset>");

            var datasetElementA = DatasetElement.Load(elementA);
            var datasetElementB = DatasetElement.Load(elementB);

            try
            {
                DbSafeManagerHelper.CompareDatasets(datasetElementA, datasetElementB, new string[] { "key" }, false);
            }
            catch (AssertFailedException ex)
            {
                StringAssert.Contains(ex.Message, "Expected Data is null but actual Data is not null");
                return;
            }

            Assert.Fail("An exception was not thrown");
        }

        [TestMethod]
        public void CompareDatasets_Given_not_null_expected_data_and_null_actual_data_Method_fails()
        {
            var elementA = XElement.Parse(@"<dataset name=""A"" table=""tableA""><data/></dataset>");
            var elementB = XElement.Parse(@"<dataset name=""B"" table=""tableA""></dataset>");

            var datasetElementA = DatasetElement.Load(elementA);
            var datasetElementB = DatasetElement.Load(elementB);

            try
            {
                DbSafeManagerHelper.CompareDatasets(datasetElementA, datasetElementB, new string[] { "key" }, false);
            }
            catch (AssertFailedException ex)
            {
                StringAssert.Contains(ex.Message, "Expected Data is not null but actual Data is null");
                return;
            }

            Assert.Fail("An exception was not thrown");
        }

        [TestMethod]
        public void CompareDatasets_Given_different_number_of_rows_Method_fails()
        {
            var elementA = XElement.Parse(@"<dataset name=""A"" table=""tableA""><data/></dataset>");
            var elementB = XElement.Parse(@"
<dataset name=""B"" table=""tableA"">
    <data>
        <row/>
    </data>
</dataset>
");

            var datasetElementA = DatasetElement.Load(elementA);
            var datasetElementB = DatasetElement.Load(elementB);

            try
            {
                DbSafeManagerHelper.CompareDatasets(datasetElementA, datasetElementB, new string[] { "key" }, false);
            }
            catch (AssertFailedException ex)
            {
                StringAssert.Contains(ex.Message, "The number of rows are different.");
                return;
            }

            Assert.Fail("An exception was not thrown");
        }

        [TestMethod]
        public void CompareDatasets_Given_datasets_with_same_data_Method_succeed()
        {
            var element = XElement.Parse(@"
<dataset name=""B"" table=""tableA"">
    <data>
        <row id=""10"" name=""AA""/>
        <row id=""11"" name=""BB""/>
    </data>
</dataset>
");

            var datasetElementA = DatasetElement.Load(element);
            var datasetElementB = DatasetElement.Load(element);

            DbSafeManagerHelper.CompareDatasets(datasetElementA, datasetElementB, new string[] { "id" }, false);
        }

        [TestMethod]
        public void CompareDatasets_Given_datasets_with_same_unsorted_data_Method_fails()
        {
            var elementA = XElement.Parse(@"
<dataset name=""B"" table=""tableA"">
    <data>
        <row id=""10"" name=""AA""/>
        <row id=""11"" name=""BB""/>
    </data>
</dataset>
");

            var elementB = XElement.Parse(@"
<dataset name=""B"" table=""tableA"">
    <data>
        <row id=""11"" name=""BB""/>
        <row id=""10"" name=""AA""/>
    </data>
</dataset>
");

            var datasetElementA = DatasetElement.Load(elementA);
            var datasetElementB = DatasetElement.Load(elementB);

            try
            {
                DbSafeManagerHelper.CompareDatasets(datasetElementA, datasetElementB, new string[] { "id" }, true);
            }
            catch (AssertFailedException)
            {
                return;
            }

            Assert.Fail("An exception was not thrown");
        }
    }
}
