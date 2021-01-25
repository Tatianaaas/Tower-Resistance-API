using System;
using System.Collections.ObjectModel;
using System.Data;
using JogoApi.Interfaces;
using JogoApi.Models;
using Microsoft.Data.SqlClient;

namespace JogoApi.DataAccess
{
    public class UserDAO : IDAO<User>, IDisposable
    {
        private IConnection _connection;

        public UserDAO(IConnection connection)
        {
            this._connection = connection;
        }
        public User Create(User model)
        {
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "insert into Users (Username, PasswordHash, PassworSalt, Role, Name, Email, Activated) values(@un, @ph, @ps, @ro, @na, @em, @ac); Select @@Identity";
                cmd.Parameters.Add("@un", SqlDbType.VarChar).Value = model.Username;
                cmd.Parameters.Add("@ph", SqlDbType.VarBinary).Value = model.PasswordHash;
                cmd.Parameters.Add("@ps", SqlDbType.VarBinary).Value = model.PasswordSalt;
                cmd.Parameters.Add("@ro", SqlDbType.VarChar).Value = model.Role;
                cmd.Parameters.Add("@na", SqlDbType.VarChar).Value = model.Name;
                cmd.Parameters.Add("@em", SqlDbType.VarChar).Value = model.Email;
                cmd.Parameters.Add("@ac", SqlDbType.Bit).Value = model.Activated;

                model.Id = int.Parse(cmd.ExecuteScalar().ToString());
            }
            return model;
        }

        public bool Delete(User model)
        {
            bool deleted = false;
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "delete from Users where Id=@id";
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = model.Id;
                if (cmd.ExecuteNonQuery() > 0)
                {
                    deleted = true;
                }
            }
            return deleted;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public User FindByID(params object[] keys)
        {
            User user = null;
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT Id, Username FROM Users where Id=@id ";
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = keys[0];
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        user = new User();
                        reader.Read();
                        user.Id = reader.GetInt32(0);
                        user.Username = reader.GetString(1);
                        // user.PasswordHash = reader.GetByte(2);
                        // user.PasswordSalt = reader.GetBytes(3);
                        //user.Role = reader.GetString(3);
                        // user.Token = reader.Ge(4);
                    }
                }
            }
            return user;
        }

        public System.Collections.ObjectModel.Collection<User> GetAll()
        {
            Collection<User> users = new Collection<User>();
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT Id, Username, Role FROM Users ORDER BY Id";
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    foreach (DataRow row in table.Rows)
                    {
                        User player = new User
                        {
                            Id = int.Parse(row["Id"].ToString()),
                            Username = row["Username"].ToString(),
                            Role = row["Role"].ToString(),
                        };
                        users.Add(player);
                    }
                }
            }
            return users;
        }

        public Collection<User> GetAllData()
        {
            throw new NotImplementedException();
        }

        public void Update(User model)
        {
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "update Users set Username=@un, PasswordHash=@ph, PasswordSalt=@ps, Role=@ro, Email=@em, Activated=@ac where Id=@id ";
                //cmd.Parameters.Add("@id", SqlDbType.Int).Value = model.Id;
                cmd.Parameters.Add("@un", SqlDbType.VarChar).Value = model.Username;
                cmd.Parameters.Add("@ph", SqlDbType.VarBinary).Value = model.PasswordHash;
                cmd.Parameters.Add("@ps", SqlDbType.VarBinary).Value = model.PasswordHash;
                cmd.Parameters.Add("@ro", SqlDbType.VarChar).Value = model.Role;
                cmd.Parameters.Add("@em", SqlDbType.VarChar).Value = model.Email;
                cmd.Parameters.Add("@ac", SqlDbType.Bit).Value = model.Activated; 

                cmd.ExecuteNonQuery();
            }

        }
    }
}