using System;
using System.Data;
using JogoApi.Interfaces;
using Microsoft.Data.SqlClient;

namespace JogoApi.DataAccess{
    public class Connection : IConnection, IDisposable
    {

        private SqlConnection _connection;

        public Connection(){
            _connection = new SqlConnection("Data Source=LAPTOP-E3TS56E0; Initial Catalog=JogoLDS ; Integrated Security=SSPI");
        }
        public void Close()
        {
            if(_connection.State== ConnectionState.Open){
                _connection.Close();
            }
        }

        public void Dispose()
        {
           this.Close();
           GC.SuppressFinalize(this);
        }

        public SqlConnection Fetch()
        {
           return this.Open();
        }

        public SqlConnection Open()
        {
            if(_connection.State== ConnectionState.Closed){
            
                _connection.Open();
            }
            return _connection;
        }

    }
}