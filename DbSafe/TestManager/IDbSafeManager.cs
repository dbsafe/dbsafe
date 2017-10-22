using System;

namespace DbSafe
{
    public interface IDbSafeManager
    {
        DbSafeManagerConfig Config { get; }

        IDbSafeManager ExecuteScripts(params string[] scriptNames);
        IDbSafeManager LoadTables(params string[] datasetNames);

        IDbSafeManager RegisterFormatter(Type type, Func<object, string> func);
        IDbSafeManager RegisterFormatter(Type type, IColumnFormatter formatter);

        IDbSafeManager RegisterFormatter(string tableName, string columnName, Func<object, string> func);
        IDbSafeManager RegisterFormatter(string tableName, string columnName, IColumnFormatter formatter);

        IDbSafeManager RegisterFormatter(string columnName, Func<object, string> func);
        IDbSafeManager RegisterFormatter(string columnName, IColumnFormatter formatter);

        void Completed();
        void AssertDatasetVsScript(string expectedDatasetName, string actualScriptName, bool sorted, string key, params string[] otherKeys);
        void AssertDatasetVsScript(string expectedDatasetName, string actualScriptName, string key, params string[] otherKeys);
    }
}
