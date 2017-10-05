using System;

namespace DbSafe
{
    public interface IDbSafeManager
    {
        DbSafeManagerConfig Config { get; }

        IDbSafeManager ExecuteScripts(params string[] scriptNames);
        IDbSafeManager LoadTables(params string[] datasetNames);
        IDbSafeManager RegisterFormatter(Type type, Func<object, string> func);
        IDbSafeManager RegisterFormatter(string tableName, string columnName, Func<object, string> func);
        IDbSafeManager RegisterFormatter(string columnName, Func<object, string> func);

        void Completed();
        void AssertDatasetVsScript(string expectedDatasetName, string actualScriptName, bool sorted, params string[] keys);
        void AssertDatasetVsScript(string expectedDatasetName, string actualScriptName, params string[] keys);
    }
}
