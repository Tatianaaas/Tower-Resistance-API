using System;
using System.Collections.ObjectModel;
using System.Data;
using JogoApi.Interfaces;
using JogoApi.Models;
using Microsoft.Data.SqlClient;

namespace JogoApi.DataAccess
{
    public class FriendDAO : IDAO<Friend>, IDisposable
    {
        private IConnection _connection;

        public FriendDAO(IConnection connection)
        {
            this._connection = connection;
        }
        public Friend Create(Friend model)
        {
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "insert into Friends (idInvite, idUser, idUserFriend) values(@iv,@iu, @if); Select @@Identity";
                cmd.Parameters.Add("@iv", SqlDbType.Int).Value = model.IdInvite;
                cmd.Parameters.Add("@iu", SqlDbType.Int).Value = model.UserId;
                cmd.Parameters.Add("@if", SqlDbType.Int).Value = model.FriendUserId;
                model.Id = int.Parse(cmd.ExecuteScalar().ToString());
            }
            return model;
        }

        public bool Delete(Friend model)
        {
            bool deleted = false;
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "delete from Friends where Id=@id";
                cmd.Parameters.Add("@iv", SqlDbType.Int).Value = model.UserId;
                cmd.Parameters.Add("@iu", SqlDbType.Int).Value = model.UserId;
                cmd.Parameters.Add("@if", SqlDbType.Int).Value = model.FriendUserId;
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

        public Friend FindByID(params object[] keys)
        {
            Friend friend = null;
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT Id ,idInvite, idUser, idUserFriend FROM Friends where Id=@id ";
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = keys[0];
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        friend = new Friend();
                        reader.Read();
                        friend.IdInvite = reader.GetInt32(1);
                        friend.UserId = reader.GetInt32(2);
                        friend.FriendUserId = reader.GetInt32(3);
                    }
                }
            }
            return friend;
        }

        public System.Collections.ObjectModel.Collection<Friend> GetAll()
        {
            Collection<Friend> friends1 = new Collection<Friend>();
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT Friends.Id, Friends.idInvite, Friends.idUser, Friends.idUserFriend FROM Friends INNER JOIN  Invites ON Friends.idInvite = Invites.id LEFT OUTER JOIN Users ON Friends.idUser = Users.id AND Friends.idUserFriend = Users.id where Invites.Status = 1";
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    foreach (DataRow row in table.Rows)
                    {
                        Friend player = new Friend
                        {
                            Id = int.Parse(row["Id"].ToString()),
                            IdInvite = int.Parse(row["idInvite"].ToString()),
                            UserId = int.Parse(row["idUser"].ToString()),
                            FriendUserId = int.Parse(row["idUserFriend"].ToString()),
                        };
                        friends1.Add(player);
                    }
                }
            }
            return friends1;
        }

        
        public System.Collections.ObjectModel.Collection<Friend> GetAllData()
        {
            Collection<Friend> friends1 = new Collection<Friend>();
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT Id, idInvite, idUser, idUserFriend FROM Friends";
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    foreach (DataRow row in table.Rows)
                    {
                        Friend player = new Friend
                        {
                            Id = int.Parse(row["Id"].ToString()),
                            IdInvite = int.Parse(row["idInvite"].ToString()),
                            UserId = int.Parse(row["idUser"].ToString()),
                            FriendUserId = int.Parse(row["idUserFriend"].ToString()),
                        };
                        friends1.Add(player);
                    }
                }
            }
            return friends1;
        }

        public void Update(Friend model)
        {
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "update Friends set idInvite=@iv, idUser=@iu, idUserFriend=@if ";
                cmd.Parameters.Add("@iv", SqlDbType.Int).Value = model.UserId;
                cmd.Parameters.Add("@iu", SqlDbType.Int).Value = model.UserId;
                cmd.Parameters.Add("@if", SqlDbType.Int).Value = model.FriendUserId;

                cmd.ExecuteNonQuery();
            }

        }
    }
}