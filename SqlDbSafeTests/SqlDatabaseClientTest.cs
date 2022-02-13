using DbSafe;
using DbSafe.FileDefinition;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlDbSafe;
using System.Xml.Linq;

namespace SqlDbSafeTests
{
    // This test uses a MS-SQL Server database from
    // https://github.com/dbsafe/dbsafe-sql-db
    [TestClass]
    public class SqlDatabaseClientTest
    {
        private readonly string _connectionString = @"data source=(localdb)\ProjectModels;initial catalog=ProductDatabase;integrated security=True;MultipleActiveResultSets=True;App=SqlDatabaseClientTest";

        private readonly string _createGlobalTempTableCommand = @"
            IF OBJECT_ID('tempdb.dbo.##GlobalTempTable') IS NOT NULL
            BEGIN
                DROP TABLE ##GlobalTempTable
            END;

            CREATE TABLE ##GlobalTempTable (col1 INT PRIMARY KEY);
";

        private readonly string _verifyIfTempTableExistsQuery = @"
            IF OBJECT_ID('tempdb.dbo.##GlobalTempTable') IS NOT NULL
                SELECT '1' AS TempTableExists
            ELSE
                SELECT '0' AS TempTableExists;
";

        [TestMethod]
        public void Write_read_and_compare_records()
        {
            var target = new SqlDatabaseClient(false) { ConnectionString = _connectionString };

            var deleteDataCommand = @"
DELETE [dbo].[Product];
DELETE [dbo].[Category];
DELETE [dbo].[Supplier];
";
            target.ExecuteCommand(deleteDataCommand);

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

            target.WriteTable(dataset);

            var formatterManager = new FormatterManager();

            var selectRecordsQuery = "SELECT * FROM [dbo].[Supplier];";

            var actual = target.ReadTable(selectRecordsQuery, formatterManager);

            DbSafeManagerHelper.CompareDatasets(dataset, actual, new string[] { "Id" }, false, false);
        }

        [TestMethod]
        public void Connection_is_not_reused()
        {
            // This test uses a global temp table (GTT) since a GTT is deleted when the connection that created it goes out of scope.
            var target = new SqlDatabaseClient(false) { ConnectionString = _connectionString };

            target.ExecuteCommand(_createGlobalTempTableCommand);

            var formatterManager = new FormatterManager();
            var actual = target.ReadTable(_verifyIfTempTableExistsQuery, formatterManager);

            var expectedDatasetXml = @"
<dataset name=""a-name"" table=""a-name"">
  <data>
    <row TempTableExists=""0"" />
  </data>
</dataset>
";
            var xml = XElement.Parse(expectedDatasetXml);
            var expectedDataset = DatasetElement.Load(xml);

            Assert.AreEqual(expectedDataset.Data.ToString(), actual.Data.ToString());
        }

        [TestMethod]
        public void Connection_is_reused_when_configured()
        {
            // This test uses a global temp table (GTT) since a GTT is deleted when the connection that created it goes out of scope.
            var target = new SqlDatabaseClient(true) { ConnectionString = _connectionString };
            target.ExecuteCommand(_createGlobalTempTableCommand);

            var formatterManager = new FormatterManager();
            var actual = target.ReadTable(_verifyIfTempTableExistsQuery, formatterManager);

            var expectedDatasetXml = @"
<dataset name=""a-name"" table=""a-name"">
  <data>
    <row TempTableExists=""1"" />
  </data>
</dataset>
";
            var xml = XElement.Parse(expectedDatasetXml);
            var expectedDataset = DatasetElement.Load(xml);

            Assert.AreEqual(expectedDataset.Data.ToString(), actual.Data.ToString());
        }
    }
}
