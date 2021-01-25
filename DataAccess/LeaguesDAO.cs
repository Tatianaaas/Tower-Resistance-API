using System;
using System.Collections.ObjectModel;
using System.Data;
using JogoApi.Interfaces;
using JogoApi.Model.Game;
using JogoApi.Models;
using Microsoft.Data.SqlClient;

namespace JogoApi.DataAccess
{
    public class LeaguesDAO : IDAO<Leagues>, IDisposable
    {
        private IConnection _connection;

        public LeaguesDAO(IConnection connection)
        {
            this._connection = connection;
        }
        public Leagues Create(Leagues model)
        {
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "insert into Leagues (Id, idUser, Description, Points) values(@id, @iu, @dp, @pt); Select @@Identity";
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = model.Id;
                cmd.Parameters.Add("@iu", SqlDbType.Int).Value = model.IdUser;
                cmd.Parameters.Add("@dp", SqlDbType.VarChar).Value = model.Description;
                cmd.Parameters.Add("@pt", SqlDbType.Int).Value = model.Points;

                model.Id = int.Parse(cmd.ExecuteScalar().ToString());
                model.IdUser = int.Parse(cmd.ExecuteScalar().ToString());
            }
            return model;
        }

        public bool Delete(Leagues model)
        {
            bool deleted = false;
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "delete from Leagues where Id=@id and idUser=@iu ";
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = model.Id;
                cmd.Parameters.Add("@iu", SqlDbType.Int).Value = model.Id;

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

        public Leagues FindByID(params object[] keys)
        {
            Leagues league = null;
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT  Id, idUser, Description, Points  FROM Leagues where Id=@id and idUser=@iu ";
                cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = keys[0];
                cmd.Parameters.Add("@iu", SqlDbType.Int).Value = keys[1];

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        league = new Leagues();
                        reader.Read();
                        league.Id = reader.GetInt32(0);
                        league.IdUser = reader.GetInt32(1);
                        league.Description = reader.GetString(2);
                        league.Points = reader.GetInt32(3);
                    }
                }
            }
            return league;
        }

        public System.Collections.ObjectModel.Collection<Leagues> GetAll()
        {
            Collection<Leagues> leagues = new Collection<Leagues>();
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM Leagues ORDER BY Id";
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    foreach (DataRow row in table.Rows)
                    {
                        Leagues player = new Leagues
                        {
                            Id = int.Parse(row["Id"].ToString()),
                            IdUser = int.Parse(row["idUser"].ToString()),
                            Description = row["Description"].ToString(),
                            Points = int.Parse(row["Points"].ToString()),
                        };
                        leagues.Add(player);
                    }
                }
            }
            return leagues;
        }

        public Collection<Leagues> GetAllData()
        {
            throw new NotImplementedException();
        }

        public void Update(Leagues model)
        {
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "update Leagues set Description=@dp, Points=@pt where Id=@id and idUser=@iu";

                cmd.Parameters.Add("@dp", SqlDbType.VarChar).Value = model.Description;
                cmd.Parameters.Add("@pt", SqlDbType.Int).Value = model.Points;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = model.Id;
                cmd.Parameters.Add("@iu", SqlDbType.Int).Value = model.IdUser;
                cmd.ExecuteNonQuery();
            }

        }
    }
}