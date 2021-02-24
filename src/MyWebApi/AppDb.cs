using System;
using MySqlConnector;

namespace MyWebApi
{
    public class AppDb : IDisposable
    {
        public MySqlConnection Connection { get; }

        public AppDb(string connectionString)
        {
            Connection = new MySqlConnection(connectionString);
        }

        public AppDb()
		{
			Connection = new MySqlConnection(AppConfig.Config["Data:ConnectionString"]);
		}

        public void Dispose() => Connection.Dispose();
    }
}