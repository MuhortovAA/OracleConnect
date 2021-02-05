Oracle Entity Framework Core 3.19.80 README
===========================================
Release Notes: ODP.NET Entity Framework Core

August 2020

ODAC documentation can be found here:
https://docs.oracle.com/en/database/oracle/oracle-data-access-components/19.3.2/

This document provides information that supplements the ODP.NET Entity Framework Core documentation.
This product's license agreement is available at
https://www.oracle.com/downloads/licenses/distribution-license.html


Bugs Fixed since Oracle.EntityFrameworkCore NuGet Package 3.19.0-beta2
======================================================================
Bug 31726832 SCRIPT-MIGRATION WITH IDEMPOTENT OPTION GENERATES INVALID SQL SCRIPT
Bug 31423881 SPECIFIED CAST IS NOT VALID IN QUERY FOR DATETIME UNLESS VARIABLE USED
Bug 31661791 RAISE_APPLICATION_ERROR -20001 RETURNS INCORRECT "REQUIRED USER DOES NOT EXIST" ERROR
Bug 31645329 SCAFFOLDING ERROR ORA-00942: THE TABLE OR VIEW DOES NOT EXIST


Tips, Limitations, and Known Issues
===================================
Performance
-----------
The performance of the EF Core application is most optimal for binding character-based data by assuring that the mapping between the
string entity property's bind datatype and the column datatype match in terms of its Unicode support.  If the mapping is done properly,
the application will bind the string entity property value properly as NVARCHAR2 for a NVARCHAR2 column or bind it as VARCHAR2 for a
VARCHAR2 column.  If the types are mismatched, additional processing is incurred on the server side, slowing down performance.

EF Core applications can specify how the string entity property values are mapped to either VARCHAR2 or NVARCHAR2 by using
the IsUnicode() or the HasColumnType() fluent APIs.

If string entity property is associated with a VARCHAR2 column, make sure that string entity property has either
- IsUnicode(false), or
- HasColumnType("VARCHAR2(<length>)")
fluent APIs invoked to bind the data as VARCHAR2.

If string entity property is associated with a NVARCHAR2 column, make sure that string entity property has either
- No invocations of IsUnicode() or HasColumnType() fluent APIs, or
- IsUncode(true), or
- HasColumnType("NVARCHAR2(<length>)")
fluent APIs invoked to bind the data as NVARCHAR2.

IMPORTANT: With Oracle.EntityFrameworkCore 2.19.70 and earlier versions, while executing LINQ queries,
string entity property values were always bound as VARCHAR2. But with the 2.19.80/3.19.80 versions,
the string entity property values are now bound based on the mapping specified for entity string property. This means
that application that performed optimally with 2.19.70 can degrade in performance when upgrading to 2.19.80/3.19.80.
For example, if the string entity property is associated with a VARCHAR2 column, but neither
- IsUnicode(false) nor
- HasColumnType("VARCHAR2(<length>)")
fluent APIs are invoked to bind the data as VARCHAR2, be sure to modify such applications to have the character-based data
bound using the correct type using the IsUnicode(false) or HasColumnType("VARCHAR2(<length>)") fluent APIs.

Please read the documentation for more details.

Code First
----------
* The HasIndex() Fluent API cannot be invoked on an entity property that will result in a primary key in the Oracle database. 
Oracle Database does not support index creation for primary keys since an index is implicitly created for all primary keys.

* Oracle Database 11.2 does not support default expression to reference any PL/SQL functions nor any pseudocolumns such as 
'<sequence>.NEXTVAL'. As such, HasDefaultValue() and HasDefaultValueSql() Fluent APIs cannot be used in conjunction with 
'sequence.NEXTVAL' as the default value, for example, with the Oracle Database 11.2. However, the application can use the 
UseOracleIdentityColumn() extension method to have the column be populated with server generated values even for Oracle 
Database 11.2. Please read about UseOracleIdentityColumn() for more details.

* The HasFilter() Fluent API is not supported. For example, 
modelBuilder.Entity<Blog>().HasIndex(b => b.Url.HasFilter("Url is not null");

Computed Columns
----------------
* Literal values used for computed columns must be encapsulated by two single-quotes. In the example below, the literal string 
is the comma. It needs to be surrounded by two single-quotes as shown below.

     // C# - computed columns code sample
    modelBuilder.Entity<Blog>()
    .Property(b => b.BlogOwner)
    .HasComputedColumnSql("\"LastName\" || '','' || \"FirstName\"");

Database Scalar Function Mapping
--------------------------------
* Database scalar function mapping does not provide a native way to use functions residing within PL/SQL packages. To work around 
this limitation, map the package and function to an Oracle synonym, then map the synonym to the EF Core function.

LINQ
----
* Oracle Database 12.1 has the following limitation: if the select list contains columns with identical names and you specify the 
row limiting clause, then an ORA-00918 error occurs. This error occurs whether the identically named columns are in the same table 
or in different tables.

Let us suppose that database contains following two table definitions:
SQL> desc X;
 Name    Null?    Type
 ------- -------- ----------------------------
 COL1    NOT NULL NUMBER(10)
 COL2             NVARCHAR2(2000)

SQL> desc Y;
 Name    Null?    Type
 ------- -------- ----------------------------
 COL0    NOT NULL NUMBER(10)
 COL1             NUMBER(10)
 COL3             NVARCHAR2(2000)

Executing the following LINQ, for example, would generate a select query which would contain "COL1" column from both the tables. 
Hence, it would result in error ORA-00918:
dbContext.Y.Include(a => a.X).Skip(2).Take(3).ToList();
This error does not occur when using Oracle Database 11.2, 12.2, and higher versions.

* Certain LINQs cannot be executed against Oracle Database 11.2.
Let us first imagine an Entity Model with the following entities:

public class Gear
{
    public string FullName { get; set; }
    public virtual ICollection<Weapon> Weapons { get; set; }
}

public class Weapon
{
    public int Id { get; set; }
    public bool IsAutomatic { get; set; }
    public string OwnerFullName { get; set; }
    public Gear Owner { get; set; }
}

The following LINQ will not work against Oracle Database 11.2:

dbContext.Gear.Include(i => i.Weapons).OrderBy(o => o.Weapons.OrderBy(w => w.Id).FirstOrDefault().IsAutomatic).ToList();

This is due to LINQ creating the following SQL query:

SELECT "i"."FullName"
FROM "Gear" "i"
ORDER BY (
    Select
     K0 "IsAutomatic" from(
    SELECT "w"."IsAutomatic" K0
    FROM "Weapon" "w"
    WHERE ("i"."FullName" = "w"."OwnerFullName")
    ORDER BY "w"."Id" NULLS FIRST
    ) "m1"
    where rownum <= 1
) NULLS FIRST, "i"."FullName" NULLS FIRST

Within the SELECT statement, there are two nested SELECTs. The generated SQL will encounter a ORA-00904 : 
"invalid identifier" error with Oracle Database 11.2 since it has a restriction where it does not recognize outer 
select table alias "i" in the inner nested select query.

Migrations
----------
* If more than one column is associated with any sequence/trigger, then ValueGeneratedOnAdd() Fluent API will be generated 
for each of these columns when performing a scaffolding operation. If we then use this scaffolded model to perform a migration, 
then an issue occurs. Each column associated with the ValueGeneratedOnAdd() Fluent API is made an identity column by default. 
To avoid this issue, use UseOracleSQLCompatibility("11") which will force Entity Framework Core to generate triggers/sequences 
instead.

Scaffolding
-----------
* Scaffolding a table that uses Function Based Indexes is supported. However, the index will NOT be scaffolded.
* Scaffolding a table that uses Conditional Indexes is not supported.

Sequences
---------
* A sequence cannot be restarted.
* Extension methods related to SequenceHiLo is not supported, except for columns with Char, UInt, ULong, and UByte data types.


 Copyright (c) 2020, Oracle and/or its affiliates. 
