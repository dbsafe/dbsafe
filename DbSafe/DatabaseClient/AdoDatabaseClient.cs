using DbSafe.FileDefinition;
using System.Data;
using System.Data.Common;
using System.Xml.Linq;

namespace DbSafe
{
    public abstract class AdoDatabaseClient<TDbConnection, TDbCommand> : IDatabaseClient
            where TDbConnection : DbConnection
            where TDbCommand : DbCommand
    {
        public string ConnectionString { get; set; }

        public void ExecuteCommand(string command)
        {
            ExecuteCommand(command, ConnectionString);
        }

        public virtual DatasetElement ReadTable(string command, FormatterManager formatter)
        {
            DatasetElement result = new DatasetElement();
            result.Data = new XElement("data");

            using (var conn = CreateDbConnection(ConnectionString))
            {
                conn.Open();
                using (var comm = CreateDbCommand(command, conn))
                {
                    var reader = comm.ExecuteReader(CommandBehavior.KeyInfo);
                    var tableSchema = reader.GetSchemaTable();
                    result.Table = tableSchema.TableName;
                    while (reader.Read())
                    {
                        XElement xmlRow = new XElement("row");
                        result.Data.Add(xmlRow);

                        foreach (var dbColumn in reader.GetColumnSchema())
                        {
                            var value = formatter.Format(tableSchema.TableName, dbColumn.ColumnName, reader[dbColumn.ColumnOrdinal.GetValueOrDefault()]);
                            var attribute = new XAttribute(dbColumn.ColumnName, value);
                            xmlRow.Add(attribute);
                        }
                    }
                }
            }

            return result;
        }

        public abstract void WriteTable(DatasetElement dataset);

        protected void ExecuteCommand(string command, string connectionString)
        {
            using (var conn = CreateDbConnection(connectionString))
            {
                conn.Open();
                using (var comm = CreateDbCommand(command, conn))
                {
                    comm.ExecuteNonQuery();
                }
            }
        }

        protected abstract TDbConnection CreateDbConnection(string connectionString);

        protected abstract TDbCommand CreateDbCommand(string command, TDbConnection conn);
    }
}
