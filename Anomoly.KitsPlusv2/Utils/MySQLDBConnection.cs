using MySql.Data.MySqlClient;
using Rocket.Core.Logging;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anomoly.KitsPlusv2.Utils
{
    public class MySQLDBConnection : IDisposable
    {
        private readonly string _connectionString;
        internal MySqlConnection _conn;

        public MySQLDBConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        private void OpenConnection()
        {
            if (_conn == null)
                _conn = new MySqlConnection(_connectionString);

            if (_conn.State != System.Data.ConnectionState.Open)
                _conn.Open();
        }

        private MySqlCommand PrepareCommand(string query, params object[] args)
        {
            OpenConnection();

            var cmd = new MySqlCommand(query, _conn);
            int argIdx = 0;

            var sbQuery = new System.Text.StringBuilder();
            bool inString = false;

            for (int i = 0; i < query.Length; i++)
            {
                char ch = query[i];

                if (ch == '\'') inString = !inString;  // Toggle in-string state on quote

                if (ch == '?' && !inString)
                {
                    var argName = $"@p{++argIdx}";
                    var argVal = args[argIdx - 1];
                    cmd.Parameters.AddWithValue(argName, argVal);

                    sbQuery.Append(argName);
                }
                else
                {
                    sbQuery.Append(ch);
                }
            }

            cmd.CommandText = sbQuery.ToString();

#if DEBUG
            Logger.LogWarning("Query: " + cmd.CommandText);
#endif

            return cmd;
        }

        public int ExecuteUpdate(string query, params object[] parameterValues)
        {
            var cmd = PrepareCommand(query, parameterValues);
            return cmd.ExecuteNonQuery();
        }

        public MySqlDataReader Execute(string query, params object[] parameterValues)
        {
            var cmd = PrepareCommand(query, parameterValues);
            return cmd.ExecuteReader();
        }

        public async Task<int> ExecuteUpdateAsync(string query, params object[] parameterValues)
        {
            var cmd = PrepareCommand(query, parameterValues);
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<MySqlDataReader> ExecuteAsync(string query, params object[] parameterValues)
        {
            var cmd = PrepareCommand(query, parameterValues);
            return await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.Default);
        }

        public async Task<object> ExecuteScalarAsync(string query, params object[] parameterValues)
        {
            var cmd = PrepareCommand(query, parameterValues);
            return await cmd.ExecuteScalarAsync();
        }

        public void Dispose()
        {
            if (_conn != null)
            {
                try
                {
                    if (_conn.State != System.Data.ConnectionState.Closed)
                        _conn.Close();
                }
                finally
                {
                    _conn.Dispose();
                    _conn = null;
                }
            }
        }
    }

}
