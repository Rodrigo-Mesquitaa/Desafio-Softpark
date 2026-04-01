using Microsoft.Data.SqlClient;
using System.Data;

namespace Softpark.Infrastructure.Data
{
    public sealed class ConnectionFactory
    {
        private readonly string _connectionString;

        public ConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}