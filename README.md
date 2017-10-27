dbsafe – [DAL + database] integration test for .NET
==================================================================================
[![Build status](https://ci.appveyor.com/api/projects/status/vstibqep6yqfn6dr?svg=true)](https://ci.appveyor.com/project/valcarcelperez/dbsafe)

Features
--------
dbsafe provides methods for populating a database, executing SQL commands, and comparing expected data against actual data.

Supported databases
-------------------
MS SQL Server is supported by this [NuGet package](https://www.nuget.org/packages/SqlDbSafe/)

Input files
-----------
dbsafe uses one or more xml input files with SQL scripts and datasets.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<dbTest>
  <scripts>
    <script name="delete-products">
      DELETE [dbo].[Product];
    </script>

    <script name="delete-categories">
      DELETE [dbo].[Category];
    </script>
  </scripts>
  
  <datasets>
    <dataset name="categories" setIdentityInsert="true" table="Category">
      <data>
        <row Id="1" Name="category-1" />
        <row Id="2" Name="category-2" />
        <row Id="3" Name="category-3" />
      </data>
    </dataset>

    <dataset name="suppliers" setIdentityInsert="true" table="Supplier">
      <data>
        <row Id="1" Name="supplier-1" ContactName="contact-name-1" ContactPhone="100-200-0001" ContactEmail="email-1@test.com" />
        <row Id="2" Name="supplier-2" ContactName="contact-name-2" ContactPhone="100-200-0002" ContactEmail="email-2@test.com" />
        <row Id="3" Name="supplier-3" ContactName="contact-name-3" ContactPhone="100-200-0003" ContactEmail="email-3@test.com" />
      </data>
    </dataset>  
  </datasets>
</dbTest>  
```

The `<script>` elements are SQL commands that can be executed any time during the test. 
E.g. cleaning tables, selecting actual data.

The `<dataset>` elements contain data that can be used to populate a table or as the expected data. 

Initialization
--------------
The static method `Initialize` returns the DbSafeManager instance that is used during the test. One or more input files can be passed as parameters. <br>
The method `SetConnectionString` passes the name of the connection string used by DbSafeManager.
See the section [Connection String](#connection-string) for more options.

The method `ExecuteScripts` can be used to clean the tables before the test data is loaded by the method `LoadTables`.

```csharp
 [TestClass]
    public class ProductDbTest
    {
        private IDbSafeManager _dbSafe;
        
        // ...
        
        [TestInitialize]
        public void Initialize()
        {
            _dbSafe = SqlDbSafeManager.Initialize("product-db-test.xml");
            _dbSafe.SetConnectionString("ProductEntities-dbsafe");
            _dbSafe.ExecuteScripts("delete-products", "delete-categories", "delete-suppliers", "reseed-product-table");
            _dbSafe.LoadTables("categories", "suppliers", "products");
                
            // ...
        }
```

The initialization methods can be called as chainable methods.

```csharp
        [TestInitialize]
        public void Initialize()
        {
            _dbSafe = SqlDbSafeManager.Initialize("product-db-test.xml")
                .SetConnectionString("ProductEntities-dbsafe")
                .ExecuteScripts("delete-products", "delete-categories", "delete-suppliers", "reseed-product-table")
                .LoadTables("categories", "suppliers", "products");
                
            // ...
        }
```

Test Completion
---------------
Test executions are serialized by default, the method `Initialize` gets a lock and other tests will have to wait for the test that has the lock to complete. At the end of each test the method `Completed` must be called to release the lock.

The serialization of the tests is necessary to avoid tests competing for the same data when running at the same time. If the tests are designed to use different data they can run in parallel by setting `Config.SerializeTests` to `false`.

```csharp
        [TestCleanup]
        public void Cleanup()
        {
            _dbSafe?.Completed();
        }
```

Connection String
-----------------
`SetConnectionString` loads a connection string from an app.config. The connection string name must be defined in the `app.config` file.<br><br>
Starting on version 1.0.19-beta2:
<br>
`PassConnectionString` passes a full connection string, this method must be used when the project does not have an app.config file.
<br><br>
The connection string used by `SqlDbSafeManager` is an ordinal ADO.NET connection string and cannot include any specific Entity Framework (or other object-relational mapper) metadata.

Test
----
dbsafe supports writing unit tests using the AAA (Arrange, Act, Assert) pattern.

**Arrange**
initializes objects and sets the value of the data that is passed to the method under test.

Method ```ExecuteScripts``` can be used to execute scripts to delete old records.
Method ```LoadTables``` can be used to populate tables.

**Act**
invokes the method under test with the arranged parameters.

**Assert**
verifies that the action of the method under test behaves as expected.

Method ```AssertDatasetVsScript``` can be used to compare expected data vs. actual data in the database.

Simple Test
-----------
This test verifies that the method ```UpdateSupplier``` updates a record in the database. ```UpdateSupplier``` updates a supplier by its Id. The object ```supplier2``` represents the supplier with Id 2.


```csharp
        [TestMethod]
        public void UpdateSupplier_Given_supplier_Must_update_record_and_return_true()
        {
            var supplier2 = new Supplier
            {
                Id = 2,
                Name = "supplier-2-updated",
                ContactName = "contact-name-2-updated",
                ContactPhone = "100-200-9999",
                ContactEmail = "email-2-updated@test.com"
            };

            var actual = _target.UpdateSupplier(supplier2);

            Assert.IsTrue(actual);
            _dbSafe.AssertDatasetVsScript("suppliers-updated", "select-all-suppliers", "Id");
        }
```

During the initialization the table Suppliers was populated with the dataset ```suppliers```.
```xml
    <dataset name="suppliers" setIdentityInsert="true" table="Supplier">
      <data>
        <row Id="1" Name="supplier-1" ContactName="contact-name-1" ContactPhone="100-200-0001" ContactEmail="email-1@test.com" />
        <row Id="2" Name="supplier-2" ContactName="contact-name-2" ContactPhone="100-200-0002" ContactEmail="email-2@test.com" />
        <row Id="3" Name="supplier-3" ContactName="contact-name-3" ContactPhone="100-200-0003" ContactEmail="email-3@test.com" />
      </data>
    </dataset>
```

After ```UpdateSupplier(supplier2)``` is executed the method ```AssertDatasetVsScript``` asserts that the data in the dataset ```suppliers-updated``` matches the data returned by the script ```select-all-suppliers```. The column ```Id``` is used as the key value.

```xml
...
    <script name="select-all-suppliers">
      SELECT * FROM [dbo].[Supplier];
    </script>
...
    <dataset name="suppliers-updated" table="Supplier">
      <data>
        <row Id="1" Name="supplier-1" ContactName="contact-name-1" ContactPhone="100-200-0001" ContactEmail="email-1@test.com" />
        <row Id="2" Name="supplier-2-updated" ContactName="contact-name-2-updated" ContactPhone="100-200-9999" ContactEmail="email-2-updated@test.com" />
        <row Id="3" Name="supplier-3" ContactName="contact-name-3" ContactPhone="100-200-0003" ContactEmail="email-3@test.com" />
      </data>
    </dataset>    
```

Column Formatters
-----------------

Values read from a table are converted to string to create an actual local dataset. The conversion depends on the local settings.

**money, decimal**<br>
SQL Server data type `money` converts to a `string` with four decimal places, decimals are converted using the number of decimal places of the type.
e.g. `101.10` is converted to `101.1000`.

**datetime(s)**<br>
SQL Server `datatime2` converts to this format `1/1/2000 12:00:00 AM` by default.

Using custom formatters avoids having to write datasets with meaningless decimal places or dates with `00:00:00` in the time part.

Method `RegisterFormatter` registers a formatter.

Formatters:<br>
A formatter can be a type that implements the interface `IColumnFormatter` or can be a function that takes an `object` and returns a `string`.

A formatter can be registered for:

**A table name and column name**:<br>
The formatter will be used for a specific column in a specific table.<br>
**A column name**:<br>
The formatter will be used for a specific column in any table.<br>
**A type**:<br>
The formatter will be used for any columns that are of a specific type in any table.<br>

The order of precedence is:
table name and column name --> column name --> type

There are two defined formatters in dbsafe:
`DecimalFormatter` and `DateTimeFormatter`.

In this example `DateTimeFormatter` is used to format all the columns that are of type `DateTime` using the format `"yyyy-MM-dd HH:mm:ss"`
and to format all the columns called `ReleaseDate` truncating the time part. `DecimalFormatter` is used to convert all the columns that are of type `decimal` using two decimal places.

```csharp
_dbSafe.RegisterFormatter(typeof(DateTime), new DateTimeFormatter("yyyy-MM-dd HH:mm:ss"))
                .RegisterFormatter("ReleaseDate", new DateTimeFormatter("yyyy-MM-dd"))
                .RegisterFormatter(typeof(decimal), new DecimalFormatter("0.00"));
```

Database for running the tests
------------------------------
Some teams configure database tests to run against a Development database. Using a Development database makes writing tests more challenging and the build process may fail when developers are developing in the same database.

A dedicated test database used for running integration tests as part of the build process is the ideal choice.<br>
The database deployment process must run before the integration test process to ensure that the DAL and the Database are on synch.

Example Project
---------------
The repository [dbsafe-demo](https://github.com/dbsafe/dbsafe-demo) demonstrates how to use dbsafe to test a DAL component that connects to a SQL Server database.

