using System;

namespace DbSafe
{
    public class ActionFormatter : IColumnFormatter
    {
        private Func<object, string> _func;

        public ActionFormatter(Func<object, string> func)
        {
            _func = func;
        }

        public string Format(object value)
        {
            return _func(value);
        }
    }
}
