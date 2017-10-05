using DbSafe;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DbSafeTests.TestManager
{
    [TestClass]
    public class FormatterManagerTest
    {
        private FormatterManager _target;

        [TestInitialize]
        public void Initialize()
        {
            _target = new FormatterManager();

            _target.Register(typeof(DateTime), value => "registered-for-date-time");
            _target.Register("TableA", "ColumnA", value => "registered-for-TableA-ColumnA");
            _target.Register("TableA", "ColumnB", value => "registered-for-TableA-ColumnB");
            _target.Register("TableB", "ColumnA", value => "registered-for-TableB-ColumnA");
            _target.Register("ColumnA", value => "registered-for-ColumnA");
        }

        [TestMethod]
        public void Format_When_there_are_zero_registered_formatters_Format_must_return_value_to_string()
        {
            _target = new FormatterManager();

            var actual = _target.Format("TableB", "ColumnA", 5);

            Assert.AreEqual("5", actual);
        }

        [TestMethod]
        public void Format_When_a_registered_formatter_is_not_found_Method_must_return_value_to_string()
        {
            var actual = _target.Format("TableB", "ColumnB", 5);

            Assert.AreEqual("5", actual);
        }

        [TestMethod]
        public void Format_When_there_is_a_registered_formatter_for_the_table_and_column_The_formatter_must_be_used()
        {
            var actual = _target.Format("TableA", "ColumnB", 5);

            Assert.AreEqual("registered-for-TableA-ColumnB", actual);
        }

        [TestMethod]
        public void Format_When_there_is_a_registered_formatter_for_the_column_The_formatter_must_be_used()
        {
            var actual = _target.Format("TableC", "ColumnA", 5);

            Assert.AreEqual("registered-for-ColumnA", actual);
        }

        [TestMethod]
        public void Format_When_there_is_a_registered_formatter_for_the_type_The_formatter_must_be_used()
        {
            var actual = _target.Format("TableC", "ColumnC", DateTime.Now);

            Assert.AreEqual("registered-for-date-time", actual);
        }

        [TestMethod]
        public void Format_Order_of_precedence_Formatter_for_table_and_column_is_used_first()
        {
            var actual = _target.Format("TableA", "ColumnA", DateTime.Now);

            Assert.AreEqual("registered-for-TableA-ColumnA", actual);
        }

        [TestMethod]
        public void Format_Order_of_precedence_Formatter_column_is_used_second()
        {
            var actual = _target.Format("TableC", "ColumnA", DateTime.Now);

            Assert.AreEqual("registered-for-ColumnA", actual);
        }
    }
}
