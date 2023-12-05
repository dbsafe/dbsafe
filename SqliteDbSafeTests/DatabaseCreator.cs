using Microsoft.Data.Sqlite;

namespace SqliteDbSafeTests;

[TestClass]
public static class DatabaseCreator
{
    private static readonly string _createCategoryTableCommand = @"
CREATE TABLE ""Category"" (
	""Id""	INTEGER NOT NULL UNIQUE,
	""Name""	TEXT NOT NULL UNIQUE,
	PRIMARY KEY(""Id"" AUTOINCREMENT)
);";

    private static readonly string _createProductTableCommand = @"
CREATE TABLE ""Product"" (
	""Id""	INTEGER NOT NULL UNIQUE,
	""Code""	TEXT NOT NULL UNIQUE,
	""Name""	TEXT NOT NULL,
	""Description""	TEXT,
	""Cost""	INTEGER,
	""ListPrice""	INTEGER,
	""CategoryId""	INTEGER NOT NULL,
	""SupplierId""	INTEGER NOT NULL,
	""ReleaseDate""	TEXT,
	""IsActive""	INTEGER NOT NULL DEFAULT 1,
	""CreatedOn""	TEXT,
	FOREIGN KEY(""CategoryId"") REFERENCES Category(""Id""),
	FOREIGN KEY(""SupplierId"") REFERENCES Supplier(""Id""),
	PRIMARY KEY(""Id"" AUTOINCREMENT)
);";

    private static readonly string _createSupplierTableCommand = @"
CREATE TABLE ""Supplier"" (
	""Id""	INTEGER NOT NULL UNIQUE,
	""Name""	TEXT NOT NULL UNIQUE,
	""ContactName""	TEXT,
	""ContactPhone""	TEXT,
	""ContactEmail""	TEXT,
	PRIMARY KEY(""Id"" AUTOINCREMENT)
);";

	private static readonly string _dropTableCommand = "DROP TABLE IF EXISTS";

    public static string ConnectionString { get; } = "Data Source=test-database.sqlite";

    [AssemblyInitialize]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "TestContext is needed for the test to execute")]
    public static void CreateDatabase(TestContext context)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        ExecuteCommand(connection, $"{_dropTableCommand} Supplier");
        ExecuteCommand(connection, $"{_dropTableCommand} Product");
        ExecuteCommand(connection, $"{_dropTableCommand} Category");
        
        ExecuteCommand(connection, _createSupplierTableCommand);
        ExecuteCommand(connection, _createProductTableCommand);
        ExecuteCommand(connection, _createCategoryTableCommand);
    }

    private static void ExecuteCommand(SqliteConnection connection, string command)
	{
        var cmd = connection.CreateCommand();
        cmd.CommandText = command;
        cmd.ExecuteNonQuery();
    }
}
