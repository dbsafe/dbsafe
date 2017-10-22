namespace DbSafe
{
    public class DecimalFormatter : IColumnFormatter
    {
        private string _format;

        public DecimalFormatter(string format)
        {
            _format = format;
        }

        public string Format(object value)
        {
            return ((decimal)value).ToString(_format);
        }
    }
}
