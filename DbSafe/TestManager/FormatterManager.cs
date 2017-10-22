using System;
using System.Collections.Generic;

namespace DbSafe
{
    public class FormatterManager
    {
        private Dictionary<string, IColumnFormatter> _formatters = new Dictionary<string, IColumnFormatter>();

        public void Register(Type type, Func<object, string> func)
        {
            var key = BuildKeyForType(type);
            RegisterFormatterWithFunc(key, func);
        }

        public void Register(string tableName, string columnName, Func<object, string> func)
        {

            var key = BuildKeyForTableAndColumn(tableName, columnName);
            RegisterFormatterWithFunc(key, func);
        }

        public void Register(string columnName, Func<object, string> func)
        {
            var key = BuildKeyForColumn(columnName);
            RegisterFormatterWithFunc(key, func);
        }

        private void RegisterFormatterWithFunc(string key, Func<object, string> func)
        {
            _formatters[key] = new ActionFormatter(func);
        }

        public string Format(string tableName, string columnName, object value)
        {
            if (_formatters.Count == 0)
            {
                value.ToString();
            }

            var formatter = GetFormatter(tableName, columnName, value.GetType());
            var result = formatter != null ? formatter.Format(value) : value.ToString();
            return result;
        }

        private IColumnFormatter GetFormatter(string tableName, string columnName, Type type)
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

        private IColumnFormatter Find(Type type)
        {
            var key = BuildKeyForType(type);
            return FindByKey(key);
        }

        private IColumnFormatter Find(string columnName)
        {
            var key = BuildKeyForColumn(columnName);
            return FindByKey(key);
        }

        private IColumnFormatter Find(string tableName, string columnName)
        {
            var key = BuildKeyForTableAndColumn(tableName, columnName);
            return FindByKey(key);
        }

        private IColumnFormatter FindByKey(string key)
        {
            return _formatters.ContainsKey(key) ? _formatters[key] : null;
        }

        private string BuildKeyForType(Type type)
        {
            return $"formatter_type_{type.FullName}";
        }

        private string BuildKeyForTableAndColumn(string tableName, string columnName)
        {
            return $"formatter_table_{tableName}_column_{columnName}";
        }

        private string BuildKeyForColumn(string columnName)
        {
            return $"formatter_column_{columnName}";
        }
    }
}
