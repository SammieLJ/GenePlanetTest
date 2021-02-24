using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MySqlConnector;

namespace MyWebApi.Models
{
    public class LogAccessCounts
    {
        public int Id { get; set; }
        public string IP { get; set; }
        public string HostName { get; set; }
        public string Timestamp { get; set; }

		public int allCounts { get; set; }

        internal AppDb Db { get; set; }

        public LogAccessCounts()
        {
        }

        internal LogAccessCounts(AppDb db)
        {
            Db = db;
        }

        public async Task InsertAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO `LogAccessCounts` (`IP`, `HostName`) VALUES (@IP, @HostName);";
            BindParams(cmd);
            await cmd.ExecuteNonQueryAsync();
            Id = (int) cmd.LastInsertedId;
        }

		public DbCommand InsertSync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO `LogAccessCounts` (`IP`, `HostName`) VALUES (@IP, @HostName);";
            BindParams(cmd);
            cmd.ExecuteNonQuery();
            Id = (int) cmd.LastInsertedId;

			return cmd as MySqlCommand;
        }

		public DbCommand LatestCountCmd()
		{
			var cmd = Db.Connection.CreateCommand();
			cmd.CommandText = @"SELECT count(*) FROM LogAccessCounts;";
			
			return cmd as MySqlCommand;
		}

		private void BindId(MySqlCommand cmd)
		{
			cmd.Parameters.Add(new MySqlParameter
			{
				ParameterName = "@Id",
				DbType = DbType.Int32,
				Value = Id,
			});
		}

		private void BindParams(MySqlCommand cmd)
		{
			cmd.Parameters.Add(new MySqlParameter
			{
				ParameterName = "@IP",
				DbType = DbType.String,
				Value = IP,
			});
			cmd.Parameters.Add(new MySqlParameter
			{
				ParameterName = "@HostName",
				DbType = DbType.String,
				Value = HostName,
			});
		}

    }
}