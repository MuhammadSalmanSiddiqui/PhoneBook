using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace InternetPhoneBook.Helpers
{
	public class SqlHelper
	{
		static string connectionString = "";

		public static SqlConnection GetConnection()
		{
			SqlConnection conn = new SqlConnection(connectionString);
			conn.Open();
			return conn;
		}
	}
}
