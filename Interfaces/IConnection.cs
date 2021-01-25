using Microsoft.Data.SqlClient;

namespace JogoApi.Interfaces
{
    public interface IConnection
    {
        //Open DB connection
        SqlConnection Open();
        //Fetch the connection
        SqlConnection Fetch();
        //Close DB connection
        void Close();

    }
}