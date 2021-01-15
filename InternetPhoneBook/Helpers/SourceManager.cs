using InternetPhoneBook.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace InternetPhoneBook.Helpers
{
	public class SourceManager
	{
		public static int Add(PersonModel personModel)
		{
			using (var connection = SqlHelper.GetConnection())
			{
				var sqlCommand = new SqlCommand();
				sqlCommand.Connection = connection;
				sqlCommand.CommandText = @"Insert INTO People (FirstName, LastName, Phone, Email, Created)
				VALUES (@FirstName,@LastName,@Phone, @Email, @Created); SELECT CAST(scope_identity() AS int)";

				var sqlFirstNameParam = new SqlParameter
				{
					DbType = System.Data.DbType.AnsiString,
					Value = personModel.FirstName,
					ParameterName = "@FirstName"
				};

				var sqlLastNameParam = new SqlParameter
				{
					DbType = System.Data.DbType.AnsiString,
					Value = personModel.LastName,
					ParameterName = "@LastName"
				};

				var sqlPhoneParam = new SqlParameter
				{
					DbType = System.Data.DbType.AnsiString,
					Value = personModel.Phone,
					ParameterName = "@Phone"
				};
				var sqlEmailParam = new SqlParameter
				{
					DbType = System.Data.DbType.AnsiString,
					Value = personModel.Email,
					ParameterName = "@Email"
				};
				var sqlCreatedDateParam = new SqlParameter
				{
					DbType = System.Data.DbType.DateTime,
					Value = personModel.Created,
					ParameterName = "@Created"
				};

				sqlCommand.Parameters.Add(sqlFirstNameParam);
				sqlCommand.Parameters.Add(sqlLastNameParam);
				sqlCommand.Parameters.Add(sqlPhoneParam);
				sqlCommand.Parameters.Add(sqlEmailParam);
				sqlCommand.Parameters.Add(sqlCreatedDateParam);

				return (int)sqlCommand.ExecuteScalar();

			}
		}

		public static void Delete(int id)
		{
			using (var connection = SqlHelper.GetConnection())
			{
				var sqlCommand = new SqlCommand();
				sqlCommand.Connection = connection;
				sqlCommand.CommandText = @"Delete from People WHERE ID = @id;";

				var sqlIdParam = new SqlParameter
				{
					DbType = System.Data.DbType.AnsiString,
					Value = id,
					ParameterName = "@id"
				};

				sqlCommand.Parameters.Add(sqlIdParam);
				sqlCommand.ExecuteNonQuery();
			}
		}

		public static int Edit(PersonModel person)
		{
			using (var connection = SqlHelper.GetConnection())
			{
				var sqlCommand = new SqlCommand
				{
					Connection = connection,
					CommandText = @"Update People set FirstName = @FirstName, LastName = @LastName, Phone = @Phone, Email = @Email, Updated = @Updated where ID=@ID;"
				};

				var sqlIDParam = new SqlParameter
				{
					DbType = System.Data.DbType.Int32,
					Value = person.ID,
					ParameterName = "@ID"
				};

				var sqlFirstNameParam = new SqlParameter
				{
					DbType = System.Data.DbType.AnsiString,
					Value = person.FirstName,
					ParameterName = "@FirstName"
				};

				var sqlLastNameParam = new SqlParameter
				{
					DbType = System.Data.DbType.AnsiString,
					Value = person.LastName,
					ParameterName = "@LastName"
				};

				var sqlPhoneParam = new SqlParameter
				{
					DbType = System.Data.DbType.AnsiString,
					Value = person.Phone,
					ParameterName = "@Phone"
				};
				var sqlEmailParam = new SqlParameter
				{
					DbType = System.Data.DbType.AnsiString,
					Value = person.Email,
					ParameterName = "@Email"
				};
				var sqlCreatedDateParam = new SqlParameter
				{
					DbType = System.Data.DbType.DateTime,
					Value = DateTime.Now,
					ParameterName = "@Updated"
				};

				sqlCommand.Parameters.Add(sqlFirstNameParam);
				sqlCommand.Parameters.Add(sqlLastNameParam);
				sqlCommand.Parameters.Add(sqlPhoneParam);
				sqlCommand.Parameters.Add(sqlEmailParam);
				sqlCommand.Parameters.Add(sqlCreatedDateParam);
				sqlCommand.Parameters.Add(sqlIDParam);


				int id = (int)sqlCommand.ExecuteNonQuery();
				return id;
			}
		}

		public static int NumberAllRows()
		{
			using (var connection = SqlHelper.GetConnection())
			{
				var sqlCommand = new SqlCommand();
				sqlCommand.Connection = connection;
				sqlCommand.CommandText = @"Select COUNT(ID) from People;";

				return (int)sqlCommand.ExecuteScalar();
			}
		}

		public static void Update(PersonModel personModel)
		{
			//todo
		}

		public static PersonModel GetByID(int id)
		{

			using (var connection = SqlHelper.GetConnection())
			{
				var sqlCommand = new SqlCommand();
				sqlCommand.Connection = connection;
				sqlCommand.CommandText = "SELECT * FROM People Where ID = @id;";

				var sqlIdParam = new SqlParameter
				{
					DbType = System.Data.DbType.Int32,
					Value = id,
					ParameterName = "@id"
				};
				sqlCommand.Parameters.Add(sqlIdParam);

				var data = sqlCommand.ExecuteReader();
				PersonModel person = new PersonModel();

				if(data.HasRows && data.Read())
				{
					person.ID = id;
					person.FirstName = data["FirstName"].ToString();
					person.LastName = data["LastName"].ToString();
					person.Phone = data["Phone"].ToString();
					person.Email = data["Email"].ToString();
					person.Created = (DateTime)data["Created"];
				}
				return person;
			}
		}

		public static List<PersonModel> GetByName(string Name, out int num, int page = -1)
		{
			num = 0;
			using (var connection = SqlHelper.GetConnection())
			{
				List<PersonModel> persons = new List<PersonModel>();

				var sqlCommand = new SqlCommand();
				sqlCommand.Connection = connection;
				sqlCommand.CommandText = "SELECT * FROM People Where FirstName like @Name;";

				var sqlIdParam = new SqlParameter
				{
					DbType = System.Data.DbType.String,
					Value = Name += "%",
					ParameterName = "@Name"
				};
				sqlCommand.Parameters.Add(sqlIdParam);

				var data = sqlCommand.ExecuteReader();
				PersonModel person = new PersonModel();

				while (data.HasRows && data.Read())
				{
					persons.Add(new PersonModel((int)data["ID"],
					data["FirstName"].ToString(),
					data["LastName"].ToString(),
					data["Phone"].ToString(),
					data["Email"].ToString(),
					(DateTime)data["Created"],
					data["Updated"].ToString()
					));
				}

				if (page == -1)
					return persons;
				
				num = persons.Count;
				List<PersonModel> personsPag = new List<PersonModel>();
				for (int i = (page - 1) * 3; i <= (page - 1) * 3 + 2; i++)
				{
					if (i <= persons.Count - 1)
						personsPag.Add(persons[i]);
				}
				return personsPag;
			}
		}

		public static List<PersonModel> Get(int start, int take, out int num)
		{
			var personList = new List<PersonModel>();

			using (var connection = SqlHelper.GetConnection())
			{
				var sqlCommand = new SqlCommand();
				sqlCommand.Connection = connection;
				sqlCommand.CommandText = "SELECT * FROM People ORDER BY ID OFFSET @Start ROWS FETCH NEXT @Take ROWS ONLY;";

				var sqlStartParam = new SqlParameter
				{
					DbType = System.Data.DbType.Int32,
					Value = (start - 1) * take,
					ParameterName = "@Start"
				};

				var sqlTakeParam = new SqlParameter
				{
					DbType = System.Data.DbType.Int32,
					Value = take,
					ParameterName = "@Take"
				};
				sqlCommand.Parameters.Add(sqlStartParam);
				sqlCommand.Parameters.Add(sqlTakeParam);

				var data = sqlCommand.ExecuteReader();

				while (data.HasRows && data.Read())
				{
					personList.Add(new PersonModel((int)data["ID"],
					data["FirstName"].ToString(),
					data["LastName"].ToString(),
					data["Phone"].ToString(),
					data["Email"].ToString(),
					(DateTime)data["Created"],
					data["Updated"].ToString()
					));
				}
			}
			num = personList.Count;
			if (personList.Count == 0) num = -1;
			return personList;
		}
	}
}
