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

Connection String
-----------------
Must be defined in the `app.config` file.
The connection string used by `SqlDbSafeManager` is an ordinal ADO.NET connection string and cannot include any specific Entity Framework (or other object-relational mapper) metadata.


Example
-------
The repository [dbsafe-demo](https://github.com/dbsafe/dbsafe-demo) demonstrates how to use dbsafe to test a DAL component that connects to a SQL Server database.

