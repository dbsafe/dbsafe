using DbSafe;
using DbSafe.FileDefinition;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PgDbSafe;
using System.Xml.Linq;

namespace PgDbSafeTests
{
    [TestClass]
    public class PgDatabaseClientTest
    {
        private PgDatabaseClient _target;

        [TestInitialize]
        public void Initialize()
        {
            _target = new PgDatabaseClient();
            _target.ConnectionString = "Host=localhost;Database=product;Username=dbsafe;Password=dbsafe";

            var deleteDataCommand = @"
DELETE FROM public.product;
DELETE FROM public.category;
DELETE FROM public.supplier;
";
            _target.ExecuteCommand(deleteDataCommand);
        }

        // [Ignore("Uses Product_Build database from dbsafe-demo repo")]
        [TestMethod]
        public void Write_read_and_compare_records()
        {
            var datasetXml = @"
<dataset name=""suppliers"" setIdentityInsert=""true"" table=""Supplier"">
  <data>
    <row id=""1"" name=""supplier-1"" contact_name=""contact-name-1"" contact_phone=""100-200-0001"" contact_email=""email-1@test.com"" />
    <row id=""2"" name=""supplier-2"" contact_name=""contact-name-2"" contact_phone=""100-200-0002"" contact_email=""email-2@test.com"" />
    <row id=""3"" name=""supplier-3"" contact_name=""contact-name-3"" contact_phone=""100-200-0003"" contact_email=""email-3@test.com"" />
  </data>
</dataset>
";
            var xml = XElement.Parse(datasetXml);
            var dataset = DatasetElement.Load(xml);

            _target.WriteTable(dataset);

            var formatterManager = new FormatterManager();

            var selectRecordsQuery = "SELECT * FROM public.supplier;";

            var actual = _target.ReadTable(selectRecordsQuery, formatterManager);

            DbSafeManagerHelper.CompareDatasets(dataset, actual, new string[] { "id" }, false, false);
        }
    }
}
