using DbSafe;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DbSafeTests.Formatters
{
    [TestClass]
    public class DateTimeFormatterTest
    {
        private DateTime _dateTime = new DateTime(2000, 1, 2, 3, 4, 5);
        private DateTime _date = new DateTime(2001, 1, 2);

        [TestMethod]
        public void Format_Given_one_format_It_must_be_used()
        {
            var target = new DateTimeFormatter("yyyy-MM-dd HH:mm");

            var actual = target.Format(_date);
            Assert.AreEqual("2001-01-02 00:00", actual);

            actual = target.Format(_dateTime);
            Assert.AreEqual("2000-01-02 03:04", actual);
        }

        [TestMethod]
        public void Format_Given_two_formats_They_must_be_used()
        {
            var target = new DateTimeFormatter("yyyy-MM-dd HH:mm", "yyyy-MM-dd");

            var actual = target.Format(_date);
            Assert.AreEqual("2001-01-02", actual);

            actual = target.Format(_dateTime);
            Assert.AreEqual("2000-01-02 03:04", actual);
        }
    }
}
