using System;
using System.Collections.ObjectModel;
using System.Data;
using JogoApi.Interfaces;
using JogoApi.Model.Game;
using JogoApi.Models;
using Microsoft.Data.SqlClient;

namespace JogoApi.DataAccess
{
    public class MatchDAO : IDAO<MatchHistory>, IDisposable
    {
        private IConnection _connection;

        public MatchDAO(IConnection connection)
        {
            this._connection = connection;
        }
        public MatchHistory Create(MatchHistory model)
        {
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "insert into MatchHistory (Id, idUser, Date, Kills, Deaths, TowersDestroyed, GameTime, Result) values(@id,@iu, @dt, @kl, @dh, @td, @gt, @rs); Select @@Identity";
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = model.Id;
                cmd.Parameters.Add("@iu", SqlDbType.Int).Value = model.IdUser;
                cmd.Parameters.Add("@dt", SqlDbType.Date).Value = model.Date;
                cmd.Parameters.Add("@kl", SqlDbType.Int).Value = model.Kills;
                cmd.Parameters.Add("@dh", SqlDbType.Int).Value = model.Deaths;
                cmd.Parameters.Add("@td", SqlDbType.Int).Value = model.TowersDestroyed;
                cmd.Parameters.Add("@gt", SqlDbType.Time).Value = model.GameTime;
                cmd.Parameters.Add("@rs", SqlDbType.Bit).Value = model.Result;
                model.Id = int.Parse(cmd.ExecuteScalar().ToString());
                model.IdUser= int.Parse(cmd.ExecuteScalar().ToString());

            }
            return model;
        }

        public bool Delete(MatchHistory model)
        {
            bool deleted = false;
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "delete from MatchHistory where idMatchHistory=@id and idUser=@iu";
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

        public MatchHistory FindByID(params object[] keys)
        {
            MatchHistory matchHistory = null;
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT  *  FROM MatchHistory where idMatchHistory=@id and idUser=@iu";
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = keys[0];
                cmd.Parameters.Add("@iu", SqlDbType.Int).Value = keys[1];


                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        matchHistory = new MatchHistory();
                        reader.Read();
                        matchHistory.Id = reader.GetInt32(0);
                        matchHistory.IdUser = reader.GetInt32(1);
                        matchHistory.Date = reader.GetDateTime(2);
                        matchHistory.Kills = reader.GetInt32(3);
                        matchHistory.Deaths = reader.GetInt32(4);
                        matchHistory.TowersDestroyed = reader.GetInt32(5);
                        matchHistory.GameTime = reader.GetTimeSpan(6);
                        matchHistory.Result = (bool)reader.GetSqlBoolean(7);
                    }
                }
            }
            return matchHistory;
        }

        public System.Collections.ObjectModel.Collection<MatchHistory> GetAll()
        {
            Collection<MatchHistory> matchHistories = new Collection<MatchHistory>();
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT Id, idUser, Date, Kills, Deaths, TowersDestroyed, GameTime, Result FROM MatchHistory ORDER BY Id";
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    foreach (DataRow row in table.Rows)
                    {
                        MatchHistory match = new MatchHistory
                        {
                            Id =int.Parse(row["Id"].ToString()),
                            IdUser = int.Parse(row["idUser"].ToString()),
                            Date = Convert.ToDateTime(row["Date"].ToString()),
                            Kills = int.Parse(row["Kills"].ToString()),
                            Deaths = int.Parse(row["Deaths"].ToString()),
                            TowersDestroyed = int.Parse(row["TowersDestroyed"].ToString()),
                            GameTime = TimeSpan.Parse(row["GameTime"].ToString()),
                            Result = Boolean.Parse(row["Result"].ToString()),
                        };
                        matchHistories.Add(match);
                    }
                }
            }
            return matchHistories;
        }

        public Collection<MatchHistory> GetAllData()
        {
            throw new NotImplementedException();
        }

        public void Update(MatchHistory model)
        {
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "update MatchHistory set GameTime=@gt, Result=@rs where idMatchHistory=@id and idUser=@iu";

                cmd.Parameters.Add("@gt", SqlDbType.Time).Value = model.GameTime;
                cmd.Parameters.Add("@rs", SqlDbType.Bit).Value = model.Result;

                cmd.ExecuteNonQuery();
            }

        }
    }
}