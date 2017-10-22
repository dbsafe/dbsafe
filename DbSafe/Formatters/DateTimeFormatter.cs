using System;
using System.Collections.Generic;
using System.Text;

namespace DbSafe.Formatters
{
    public class DateTimeFormatter : IColumnFormatter
    {
        private string _dateWithTimeFormat;
        private string _dateWithoutTimeFormat;

        public DateTimeFormatter(string dateWithTimeFormat)
        {
            _dateWithTimeFormat = dateWithTimeFormat;
        }

        public DateTimeFormatter(string dateWithTimeFormat, string dateWithoutTimeFormat)
        {
            _dateWithTimeFormat = dateWithTimeFormat;
            _dateWithoutTimeFormat = dateWithoutTimeFormat;
        }

        public string Format(object value)
        {
            var date = (DateTime)value;
            if (date.TimeOfDay != TimeSpan.Zero)
            {
                return date.ToString(_dateWithTimeFormat);
            }

            if (string.IsNullOrWhiteSpace(_dateWithoutTimeFormat))
            {
                return date.ToString(_dateWithTimeFormat);
            }
            else
            {
                return date.ToString(_dateWithoutTimeFormat);
            }
        }
    }
}
