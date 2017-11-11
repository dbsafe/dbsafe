using DbSafe;
using DbSafe.FileDefinition;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlDbSafe;
using System.Xml.Linq;

namespace SqlDbSafeTests
{
    [TestClass]
    public class SqlDatabaseClientTest
    {
        private SqlDatabaseClient _target;

        [TestInitialize]
        public void Initialize()
        {
            _target = new SqlDatabaseClient();
            _target.ConnectionString = "data source=localhost;initial catalog=Product_Build;integrated security=True;MultipleActiveResultSets=True;App=SqlDatabaseClientTest";

            var deleteDataCommand = @"
DELETE [dbo].[Product];
DELETE [dbo].[Category];
DELETE [dbo].[Supplier];
";
            _target.ExecuteCommand(deleteDataCommand);
        }

        [Ignore("Uses Product_Build database from dbsafe-demo repo")]
        [TestMethod]
        public void Write_read_and_compare_records()
        {
            var datasetXml = @"
<dataset name=""suppliers"" setIdentityInsert=""true"" table=""Supplier"">
  <data>
    <row Id=""1"" Name=""supplier-1"" ContactName=""contact-name-1"" ContactPhone=""100-200-0001"" ContactEmail=""email-1@test.com"" />
    <row Id=""2"" Name=""supplier-2"" ContactName=""contact-name-2"" ContactPhone=""100-200-0002"" ContactEmail=""email-2@test.com"" />
    <row Id=""3"" Name=""supplier-3"" ContactName=""contact-name-3"" ContactPhone=""100-200-0003"" ContactEmail=""email-3@test.com"" />
  </data>
</dataset>
";
            var xml = XElement.Parse(datasetXml);
            var dataset = DatasetElement.Load(xml);

            _target.WriteTable(dataset);

            var formatterManager = new FormatterManager();

            var selectRecordsQuery = "SELECT * FROM [dbo].[Supplier];";

            var actual = _target.ReadTable(selectRecordsQuery, formatterManager);

            DbSafeManagerHelper.CompareDatasets(dataset, actual, new string[] { "Id" }, false, false);
        }
    }
}
