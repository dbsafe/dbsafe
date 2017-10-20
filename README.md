dbsafe – A helper library that supports writing database integration test for .NET
==================================================================================
[![Build status](https://ci.appveyor.com/api/projects/status/vstibqep6yqfn6dr?svg=true)](https://ci.appveyor.com/project/valcarcelperez/dbsafe)

Features
--------
dbsafe provides methods for populating a database, executing SQL commands, and comparing expected data against actual data.

Supported databases
-------------------
MS SQL Server is supported by this [NuGet package]( https://www.nuget.org/packages/SqlDbSafe/)

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
The static method `Initialize` returns the DbSafeManager instance that is used during the test. One or more input files can be passed as parameters. 
The method `SetConnectionString` passes the name of the connection string used by DbSafeManager.
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
Must be defined in the `app.config` file.
The connection string used by `SqlDbSafeManager` is an ordinal ADO.NET connection string and cannot include any specific Entity Framework (or other object-relational mapper) metadata.

Test
----
dbsafe supports writing unit tests using the AAA (Arrange, Act, Assert) pattern.

**Arrange**
Initializes objects and sets the value of the data that is passed to the method under test.

Method ```ExecuteScripts``` can be used to execute scripts to delete old records.
Method ```LoadTables``` can be used to populate tables.

**Act**
Invokes the method under test with the arranged parameters.

**Assert**
Verifies that the action of the method under test behaves as expected.

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

Example Project
---------------
The repository [dbsafe-demo](https://github.com/dbsafe/dbsafe-demo) demonstrates how to use dbsafe to test a DAL component that connects to a SQL Server database.

