using System;
using System.Collections.Generic;

namespace DbSafe
{
    public class FormatterManager
    {
        private Dictionary<string, Func<object, string>> _formatters = new Dictionary<string, Func<object, string>>();

        public void Register(Type type, Func<object, string> func)
        {
            var key = $"formatter_type_{type.FullName}";
            _formatters[key] = func;
        }

        public void Register(string tableName, string columnName, Func<object, string> func)
        {
            var key = $"formatter_table_{tableName}_column_{columnName}";
            _formatters[key] = func;
        }

        public void Register(string columnName, Func<object, string> func)
        {
            var key = $"formatter_column_{columnName}";
            _formatters[key] = func;
        }

        public string Format(string tableName, string columnName, object value)
        {
            if (_formatters.Count == 0)
            {
                value.ToString();
            }

            var formatter = GetFormatter(tableName, columnName, value.GetType());
            var result = formatter != null ? formatter(value) : value.ToString();
            return result;
        }

        private Func<object, string> GetFormatter(string tableName, string columnName, Type type)
        {
            var formatter = Find(tableName, columnName);
            if (formatter != null)
            {
                return formatter;
            }

            formatter = Find(columnName);
            if (formatter != null)
            {
                return formatter;
            }

            return Find(type);
        }

        private Func<object, string> Find(Type type)
        {
            var key = $"formatter_type_{type.FullName}";
            return FindByKey(key);
        }

        private Func<object, string> Find(string columnName)
        {
            var key = $"formatter_column_{columnName}";
            return FindByKey(key);
        }

        private Func<object, string> Find(string tableName, string columnName)
        {
            var key = $"formatter_table_{tableName}_column_{columnName}";
            return FindByKey(key);
        }

        private Func<object, string> FindByKey(string key)
        {
            return _formatters.ContainsKey(key) ? _formatters[key] : null;
        }
    }
}
