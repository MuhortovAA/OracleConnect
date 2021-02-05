using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
	class Program
	{
		static void Main(string[] args)
		{
			// create connection
			OracleConnection con = new OracleConnection();

			// create connection string using builder
			OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();
			ocsb.Password = "autumn117";
			ocsb.UserID = "john";
			ocsb.DataSource = "database.url:port/databasename";

			// connect
			con.ConnectionString = ocsb.ConnectionString;
			con.Open();
			Console.WriteLine("Connection established (" + con.ServerVersion + ")");
		}
	}
}
