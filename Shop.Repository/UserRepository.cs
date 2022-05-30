using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Shop.Domain.DTO;

namespace Shop.Repository
{
	public class UserRepository : BaseRepository<User>
	{
		public bool Login(string username, string password)
		{
			DataTable table = _database.GetTable($"select userid from users where username = '{username}' and password = '{password}' ");
			if (table.Rows.Count == 0)
			{
				return false;
			}
			else
				return true;
		}

        public IEnumerable<User> Load(Predicate<User> predicate)
        {
            List<User> users = new List<User>();
            var table = _database.GetTable("select * from Users Where IsDeleted = 0");

            if (table.Rows.Count == 0) throw new ArgumentNullException("Table does not exists");
            foreach (DataRow row in table.Rows)
            {
                User user = new User();
                user.ID = Convert.ToInt32(row["UserID"]);
                user.Username = Convert.ToString(row["Username"]);
                user.Password = Convert.ToString(row["Password"]);
                user.IsDeleted = Convert.ToBoolean(row["IsDeleted"]);
                users.Add(user);
            }
            return users;
        }
        public User Get(int id)
		{
			DataTable table = _database.GetTable($"select * from users where UserID = {id}");

			if (table.Rows.Count == 0) throw new ArgumentNullException("User does not exists");

			DataRow row = table.Rows[0];
			User user = new User();
			user.ID = Convert.ToInt32(row["UserID"]);
			user.Username = Convert.ToString(row["Username"]);
			user.Password = Convert.ToString(row["Password"]);
			user.IsDeleted = Convert.ToBoolean(row["IsDeleted"]);
			return user;
		}

        public override void Delete(int id)
        {
            var parameter = new SqlParameter { ParameterName = "@userID", Value = id };
            _database.ExecuteNonQuerry("DeleteUser_sp", CommandType.StoredProcedure, parameter);
        }

        public override void Insert(User user)
        {
            var parameters = new SqlParameter[]
            {
            new SqlParameter { ParameterName = "@userID", Value = user.ID },
            new SqlParameter { ParameterName = "@username", Value = user.Username },
            new SqlParameter { ParameterName = "@password", Value = user.Password },
            };
            _database.ExecuteNonQuerry("InsertUser_sp", CommandType.StoredProcedure, parameters);
        }

        public override void Update(User user)
        {
            var parameters = new SqlParameter[]
            {
            new SqlParameter { ParameterName = "@userID", Value = user.ID },
            new SqlParameter { ParameterName = "@username", Value = user.Username },
            new SqlParameter { ParameterName = "@password", Value = user.Password },
            };
            _database.ExecuteNonQuerry("UpdateUser_sp", CommandType.StoredProcedure, parameters);
        }
    }
}


