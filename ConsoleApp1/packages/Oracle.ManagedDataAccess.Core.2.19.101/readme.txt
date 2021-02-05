Oracle.ManagedDataAccess.Core NuGet Package 2.19.101 README
===========================================================
Release Notes: Oracle Data Provider for .NET Core

December 2020


New Features
============
1) .NET 5 Support
ODP.NET Core 2.19.101 supports .NET 5.

2) Allowing Deserialization of Oracle Provider Types into DataSet/DataTable
This version introduces a new AddOracleTypesDeserialization() method on the OracleConfiguration class.
This new method can be called to add ODP.NET-specific data types to the "allow" list, so that they can 
be deserialized into DataSet/DataTable.  If an attempt is made to deserialize ODP.NET-specific types 
without adding them to the "allow" list, an ODP.NET type initializer exception will be encountered.

If your app or third-party software you use adds data types to the allow list as well, be careful not to 
overwrite the ODP.NET allowed types by appending to the allow list. 

// C# snippet enbling Oracle data type deserialization, then loading them from an XML file
static void Main(string[] args)
{
    OracleConfiguration.AddOracleTypesDeserialization();
    DataSet ds = new DataSet();
    ds.ReadXml("OracleTypes.xml"); // The xml file name will depend on your application.
}

3) Bulk Copy
ODP.NET Core 19.10 now supports bulk copy, which allows fast data transfer between .NET and Oracle databases.
ODP.NET Core supports the same bulk copy APIs as unmanaged ODP.NET. These APIs are documented in the ODP.NET
Developer's Guide.

4) KeepAlive for non-Windows platforms
Starting in ODP.NET Core 19.10, KeepAlive is supported on non-Windows operating systems, such as Oracle Linux. 
For these non-Windows operating systems, .NET Core 3 or higher is required.

ODP.NET KeepAlive APIs include the OracleConnection properties: KeepAlive, KeepAliveInterval, and KeepAliveTime.

ODP.NET documentation has more details about these properties.
https://docs.oracle.com/en/database/oracle/oracle-data-access-components/19.3.2/odpnt/ConnectionKeepAlive.html#GUID-6C24C49B-5E89-4E62-BEB8-828D3B1B47D7
https://docs.oracle.com/en/database/oracle/oracle-data-access-components/19.3.2/odpnt/ConnectionKeepAliveInterval.html#GUID-ED6AE59F-F97F-4E25-B8A1-D6C0BD5CF516
https://docs.oracle.com/en/database/oracle/oracle-data-access-components/19.3.2/odpnt/ConnectionKeepAliveTime.html#GUID-CBF5E160-AE1F-4147-AA7C-2CD4E23EE105

5) Suppress GetDecimal Invalid Cast Exception
The SuppressGetDecimalInvalidCastException property has been added to the OracleDataReader and 
OracleDataAdapter classes in ODP.NET 19.10. It specifies whether to suppress the InvalidCastException and 
return a rounded-off 28 precision value if the Oracle NUMBER value has more than 28 precision.

OracleDataAdapter.SuppressGetDecimalInvalidCastException API description:
https://docs.oracle.com/en/database/oracle/oracle-data-access-components/19.3.2/odpnt/DataAdapterSuppressGetDecimalInvalidCastException.html

OracleDataReader.SuppressGetDecimalInvalidCastException API description:
https://docs.oracle.com/en/database/oracle/oracle-data-access-components/19.3.2/odpnt/DataReaderSuppressGetDecimalInvalidCastException.html


Bug Fixes since Oracle.ManagedDataAccess.Core NuGet Package 2.19.100
====================================================================
32184726 : BINARYFORMATTER NOT SUPPORTED, AFFECTING ASP.NET CORE 5 APPLICATIONS 
32285561 : THE TNS_ADMIN CONNECTION STRING ATTRIBUTE SETTING INCORRECTLY GETS RETURNED FROM ORACLECONNECTION.TNSADMIN PROPERTY GETTER 
32180725 : PLANNED OUTAGE : SERVICE DOWN WITH DRAIN_TIMEOUT : ORA-12570 ON CHECKED OUT CONNECTIONS 
32299349 : INCORRECT PARSING OF (FAILOVER=...) IN DESCRIPTION

 Copyright (c) 2020, Oracle and/or its affiliates. 
