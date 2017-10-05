using DbSafe;
using DbSafe.FileDefinition;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SqlDbSafe
{
    /// <summary>
    /// Defines a class that execute commands in a MS-SQL Server database.
    /// </summary>
    public class SqlDatabaseClient : IDatabaseClient
    {
        public string ConnectionString { get; set; }

        /// <summary>
        /// Executes a SQL command.
        /// </summary>
        /// <param name="connectionString">A connection string</param>
        /// <param name="command">A script to execute</param>
        public void ExecuteCommand(string command)
        {
            ExecuteCommand(command, ConnectionString);
        }

        public void WriteTable(DatasetElement dataset)
        {
            if (dataset.Data != null)
            {
                bool isDatabseEmpty = !dataset.Data.Elements().Any();
                if (isDatabseEmpty)
                {
                    return;
                }

                StringBuilder sb = new StringBuilder();
                if (dataset.SetIdentityInsert)
                {
                    sb.AppendFormat("SET IDENTITY_INSERT {0} ON;", dataset.Table);
                    sb.AppendLine();
                }

                foreach (var element in dataset.Data.Elements())
                {
                    var queryFieldNamesArray = element.Attributes().Select(a => a.Name.ToString()).ToArray();
                    var queryFieldNames = string.Join(", ", queryFieldNamesArray);

                    var queryFieldValuesArray = element.Attributes().Select(a => a.Value).ToArray();
                    queryFieldValuesArray = queryFieldValuesArray
                        .Select(a => string.Format("'{0}'", a))
                        .ToArray();
                    var queryFieldValues = string.Join(", ", queryFieldValuesArray);

                    var insert = string.Format("INSERT INTO {0} ({1}) VALUES ({2});", dataset.Table, queryFieldNames, queryFieldValues);
                    sb.AppendLine(insert);
                }

                if (dataset.SetIdentityInsert)
                {
                    sb.AppendFormat("SET IDENTITY_INSERT {0} OFF;", dataset.Table);
                    sb.AppendLine();
                }

                ExecuteCommand(sb.ToString());
            }
        }

        public DatasetElement ReadTable(string command, FormatterManager formatter)
        {
            DatasetElement result = new DatasetElement();
            result.Data = new XElement("data");

            using (var adp = new SqlDataAdapter(command, ConnectionString))
            {
                using (DataSet ds = new DataSet())
                {
                    adp.Fill(ds);
                    if (ds.Tables.Count == 0)
                    {
                        return result;
                    }

                    var table = ds.Tables[0];
                    result.Table = table.TableName;

                    for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
                    {
                        XElement xmlRow = new XElement("row");
                        for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++)
                        {
                            var columnName = table.Columns[columnIndex].ColumnName;
                            var value = formatter.Format(table.TableName, columnName, table.Rows[rowIndex].ItemArray[columnIndex]);
                            var attribute = new XAttribute(columnName, value);
                            xmlRow.Add(attribute);
                        }

                        result.Data.Add(xmlRow);
                    }
                }
            }

            return result;
        }

        public static void ExecuteCommand(string command, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand comm = new SqlCommand(command, conn))
                {
                    comm.ExecuteNonQuery();
                }
            }
        }
    }
}
