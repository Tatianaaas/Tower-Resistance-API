using System;
using System.Collections.ObjectModel;
using System.Data;
using JogoApi.Interfaces;
using JogoApi.Models;
using Microsoft.Data.SqlClient;

namespace JogoApi.DataAccess
{
    public class InviteDAO : IDAO<Invite>, IDisposable
    {
        private IConnection _connection;

        public InviteDAO(IConnection connection)
        {
            this._connection = connection;
        }
        public Invite Create(Invite model)
        {
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "insert into Invites ( Date, Status) values(@dt, @st); Select @@Identity";
                cmd.Parameters.Add("@dt", SqlDbType.DateTime).Value = model.Date;
                cmd.Parameters.Add("@st", SqlDbType.Bit).Value = model.Status;
                model.Id = int.Parse(cmd.ExecuteScalar().ToString());

            }
            return model;
        }

        public bool Delete(Invite model)
        {
            bool deleted = false;
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "delete from Invites where Id=@id ";
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

        public Invite FindByID(params object[] keys)
        {
            Invite invite = null;
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT  Id, Date, Status  FROM Invites where Id=@id ";
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = keys[0];

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        invite = new Invite();
                        reader.Read();
                        invite.Id = reader.GetInt32(0);
                        invite.Date = reader.GetDateTime(1);
                        invite.Status = (bool)reader.GetSqlBoolean(2);
                    }
                }
            }
            return invite;
        }

        public System.Collections.ObjectModel.Collection<Invite> GetAll()
        {
            Collection<Invite> invites = new Collection<Invite>();
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT  Id, Date, Status FROM Invites ORDER BY Id";
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    foreach (DataRow row in table.Rows)
                    {
                        Invite player = new Invite
                        {
                            Id = int.Parse(row["Id"].ToString()),
                            Date = Convert.ToDateTime(row["Date"].ToString()),
                            Status = Boolean.Parse(row["Status"].ToString()),
                        };
                        invites.Add(player);
                    }
                }
            }
            return invites;
        }

        public Collection<Invite> GetAllData()
        {
            throw new NotImplementedException();
        }

        public void Update(Invite model)
        {
            using (SqlCommand cmd = _connection.Fetch().CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "update Invites set Date=@dt, Status=@st where Id=@id";

                cmd.Parameters.Add("@dt", SqlDbType.DateTime).Value = model.Date;
                cmd.Parameters.Add("@st", SqlDbType.Bit).Value = model.Status;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = model.Id;

                cmd.ExecuteNonQuery();
            }

        }
    }
}