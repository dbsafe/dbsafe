using DbSafe;
using DbSafe.FileDefinition;
using SqliteDbSafe;
using System.Xml.Linq;

namespace SqliteDbSafeTests
{
    [TestClass]
    public class SqliteDatabaseClientTest
    {
        private readonly string _connectionString = "Data Source=test-database.sqlite";

        [TestMethod]
        public void Write_read_and_compare_records()
        {
            var target = new SqliteDatabaseClient(false) { ConnectionString = _connectionString };

            var deleteDataCommand = @"
DELETE FROM Product;
DELETE FROM Category;
DELETE FROM Supplier;
";
            target.ExecuteCommand(deleteDataCommand);

            var datasetXml = @"
<dataset name=""suppliers"" table=""Supplier"">
  <data>
    <row Id=""1"" Name=""supplier-1"" ContactName=""contact-name-1"" ContactPhone=""100-200-0001"" ContactEmail=""email-1@test.com"" />
    <row Id=""2"" Name=""supplier-2"" ContactName=""contact-name-2"" ContactPhone=""100-200-0002"" ContactEmail=""email-2@test.com"" />
    <row Id=""3"" Name=""supplier-3"" ContactName=""contact-name-3"" ContactPhone=""100-200-0003"" ContactEmail=""email-3@test.com"" />
  </data>
</dataset>
";
            var xml = XElement.Parse(datasetXml);
            var dataset = DatasetElement.Load(xml);

            target.WriteTable(dataset);

            var formatterManager = new FormatterManager();

            var selectRecordsQuery = "SELECT * FROM Supplier;";

            var actual = target.ReadTable(selectRecordsQuery, formatterManager);

            DbSafeManagerHelper.CompareDatasets(dataset, actual, new string[] { "Id" }, false, false);
        }
    }
}