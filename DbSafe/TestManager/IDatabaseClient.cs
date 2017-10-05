using DbSafe.FileDefinition;

namespace DbSafe
{
    /// <summary>
    /// Defines an object that executes SQL commands.
    /// </summary>
    public interface IDatabaseClient
    {
        /// <summary>
        /// Executes a SQL command.
        /// </summary>
        /// <param name="command">A SQL command to execute</param>
        void ExecuteCommand(string command);

        /// <summary>
        /// Populates a table with data.
        /// </summary>
        /// <param name="dataset"></param>
        void WriteTable(DatasetElement dataset);

        /// <summary>
        /// Executes a SQL command that returns a record set and builds a DatasetElement with the data.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        DatasetElement ReadTable(string command, FormatterManager formatter);
    }
}
