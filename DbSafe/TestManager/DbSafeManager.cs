using DbSafe.FileDefinition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace DbSafe
{
    public abstract class DbSafeManager<TDatabaseClient> : IDbSafeManager
        where TDatabaseClient : IDatabaseClient
    {
        private FormatterManager _formatterManager = new FormatterManager();
        private List<KeyValuePair<string, XElement>> _inputs = new List<KeyValuePair<string, XElement>>();
        private static object syncObj = new object();

        protected TDatabaseClient DatabaseClient { get; set; }

        public DbSafeManagerConfig Config { get; set; } = DbSafeManagerConfig.GlobalConfig;

        public void Completed()
        {
            EndTest();
        }

        public DbSafeManager(DbSafeManagerConfig config)
        {
            Config = config;
        }

        public IDbSafeManager LoadTables(params string[] datasetNames)
        {
            ValidateDependencies();
            foreach (var datasetName in datasetNames)
            {
                DatasetElement dataset = FindDataset(datasetName);
                DatabaseClient.WriteTable(dataset);
            }

            return this;
        }

        public IDbSafeManager ExecuteScripts(params string[] scriptNames)
        {
            ValidateDependencies();

            foreach (var scriptName in scriptNames)
            {
                ScriptElement scriptElement = FindScript(scriptName);
                DatabaseClient.ExecuteCommand(scriptElement.Value);
            }

            return this;
        }

        public void AssertDatasetVsScript(string expectedDatasetName, string actualScriptName, bool sorted, string key, params string[] otherKeys)
        {
            ValidateDependencies();

            DatasetElement expectedData = FindDataset(expectedDatasetName);
            ScriptElement actualDataScript = FindScript(actualScriptName);
            DatasetElement actualData = DatabaseClient.ReadTable(actualDataScript.Value, _formatterManager);

            string[] keys = new string[] { key };
            if (otherKeys != null)
            {
                keys = keys.Union(otherKeys).ToArray();
            }            

            DbSafeManagerHelper.CompareDatasets(expectedData, actualData, keys, sorted, false);
        }

        public void AssertDatasetVsScript(string expectedDatasetName, string actualScriptName, string key, params string[] otherKeys)
        {
            AssertDatasetVsScript(expectedDatasetName, actualScriptName, false, key, otherKeys);
        }

        public IDbSafeManager RegisterFormatter(Type type, Func<object, string> func)
        {
            _formatterManager.Register(type, func);
            return this;
        }

        public IDbSafeManager RegisterFormatter(Type type, IColumnFormatter formatter)
        {
            _formatterManager.Register(type, formatter);
            return this;
        }

        public IDbSafeManager RegisterFormatter(string columnName, Func<object, string> func)
        {
            _formatterManager.Register(columnName, func);
            return this;
        }

        public IDbSafeManager RegisterFormatter(string columnName, IColumnFormatter formatter)
        {
            _formatterManager.Register(columnName, formatter);
            return this;
        }

        protected void BeginTest()
        {
            if (Config.SerializeTests)
            {
                Monitor.Enter(syncObj);
            }
        }

        protected void EndTest()
        {
            if (Config.SerializeTests)
            {
                Monitor.Exit(syncObj);
            }
        }

        protected DbSafeManager<TDatabaseClient> Load(string filename)
        {
            var xml = XElement.Load(filename);
            var input = new KeyValuePair<string, XElement>(filename, xml);
            _inputs.Add(input);
            return this;
        }

        protected virtual void ValidateDependencies(bool allowTestWithoutInputFile = false)
        {
            if (!allowTestWithoutInputFile && _inputs.Count == 0)
            {
                throw new InvalidOperationException("Input file not specified");
            }
        }

        private ScriptElement FindScript(string scriptName)
        {
            foreach (var input in _inputs)
            {
                var scriptElement = FindScript(scriptName, input.Key, input.Value);
                if (scriptElement != null)
                {
                    return scriptElement;
                }
            }

            string message = $"Script '{scriptName}' not found";
            throw new InvalidOperationException(message);
        }

        private ScriptElement FindScript(string scriptName, string filename, XElement xml)
        {
            var script = DbSafeManagerHelper.FindChild(xml, FileDefinitionHelper.ElementNameSripts, scriptName);
            if (script != null)
            {
                return ScriptElement.Load(script, filename);
            }

            return null;
        }

        private DatasetElement FindDataset(string datasetName)
        {
            foreach (var input in _inputs)
            {
                var datasetElement = FindDataset(datasetName, input.Value);
                if (datasetElement != null)
                {
                    return datasetElement;
                }
            }

            string message = string.Format("Dataset '{0}' not found", datasetName);
            throw new InvalidOperationException(message);
        }

        private DatasetElement FindDataset(string datasetName, XElement xml)
        {
            var dataset = DbSafeManagerHelper.FindChild(xml, FileDefinitionHelper.ElementNameDatasets, datasetName);
            if (dataset != null)
            {
                return DatasetElement.Load(dataset);
            }

            return null;
        }
    }
}
