using DbSafe;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbSafeTests.Formatters
{
    [TestClass]
    public class DecimalFormatterTest
    {
        [TestMethod]
        public void Format_Test_format_with_two_decimal_places()
        {
            var value = 10.1234m;

            var target = new DecimalFormatter("0.00");

            var actual = target.Format(value);

            Assert.AreEqual("10.12", actual);
        }
    }
}
