using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace HealthAndFinance.BL
{
    public class DapperContext
    {
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DbContextConnection");
        }

        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);

        public async Task<SqlConnection> CreateAsyncConnection()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        public List<T> Query<T>(string sqlQuery, object param = null)
        {
            using var connection = CreateConnection();
            connection.Open();
            return connection.Query<T>(sqlQuery, param).ToList();
        }

        public async Task<List<T>> QueryAsync<T>(string sqlQuery, object param = null)
        {
            using var connection = CreateConnection();
            connection.Open();
            var result = await connection.QueryAsync<T>(sqlQuery, param);
            return result.ToList();
        }

        public int Execute(string sqlQuery, object param = null)
        {
            using var connection = CreateConnection();
            connection.Open();
            return connection.Execute(sqlQuery, param);
        }

        public async Task<IEnumerable<TReturn>> QueryAsync<T1, T2, TReturn>(
            string sql,
            Func<T1, T2, TReturn> map,
            object param = null,
            string splitOn = "Id")
        {
            using var connection = CreateConnection();
            connection.Open();
            return await connection.QueryAsync(sql, map, param, splitOn: splitOn);
        }

        public async Task<IEnumerable<TReturn>> QueryAsync<T1, T2, T3, TReturn>(
            string sql,
            Func<T1, T2, T3, TReturn> map,
            object param = null,
            string splitOn = "Id")
        {
            using var connection = CreateConnection();
            connection.Open();
            return await connection.QueryAsync(sql, map, param, splitOn: splitOn);
        }

        public async Task<T> ExecuteScalarAsync<T>(string sql, object param = null)
        {
            using var connection = CreateConnection();
            connection.Open();
            return await connection.ExecuteScalarAsync<T>(sql, param);
        }
        public async Task<IEnumerable<TReturn>> QueryAsync<T1, T2, T3, T4, TReturn>(
            string sql,
            Func<T1, T2, T3, T4, TReturn> map,
            object param = null,
            string splitOn = "Id")
        {
            using var connection = CreateConnection();
            connection.Open();
            return await connection.QueryAsync(sql, map, param, splitOn: splitOn);
        }

        public async Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null)
        {
            await using var connection = await CreateAsyncConnection();
            return await connection.QuerySingleOrDefaultAsync<T>(sql, param, transaction);
        }

        public async Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null)
        {
            await using var connection = await CreateAsyncConnection();
            return await connection.ExecuteAsync(sql, param, transaction);
        }
    }
}
