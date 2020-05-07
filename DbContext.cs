using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Samples.AsyncEnumerable
{
    public class DbContext
    {
        private readonly MySqlConnection _connection;

        public string ConnectionString { get; set; }

        public DbContext(string connectionString)
        {
            ConnectionString = connectionString;

            _connection = GetConnection();
            _connection.Open();
        }

        private MySqlConnection GetConnection() => new MySqlConnection(ConnectionString);

        public IEnumerable<DbModel> GetModels(int limit, int fetchSize)
        {
            var yielded = 0;

            do
            {
                using var cmd = new MySqlCommand($"SELECT Id, DayUtc, Val, MinVal, MaxVal FROM enumtest LIMIT {fetchSize};", _connection);

                using(var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return new DbModel
                                     {
                                         Id = reader.GetString(0),
                                         DayUtc = reader.GetString(1),
                                         Val = reader.GetDouble(2),
                                         MinVal = reader.GetDouble(3),
                                         MaxVal = reader.GetDouble(4),
                                     };
                    }
                }

                yielded += fetchSize;
            } while (yielded < limit);
        }

        public async IAsyncEnumerable<DbModel> GetModelsAsync(int limit, int fetchSize)
        {
            var yielded = 0;

            do
            {
                await using(var cmd = new MySqlCommand($"SELECT Id, DayUtc, Val, MinVal, MaxVal FROM enumtest LIMIT {fetchSize};", _connection))
                {
                    await using(var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await reader.ReadAsync())
                        {
                            yield return new DbModel
                                         {
                                             Id = reader.GetString(0),
                                             DayUtc = reader.GetString(1),
                                             Val = reader.GetDouble(2),
                                             MinVal = reader.GetDouble(3),
                                             MaxVal = reader.GetDouble(4),
                                         };
                        }
                    }
                }

                yielded += fetchSize;
            } while (yielded < limit);
        }

        public async IAsyncEnumerable<IEnumerable<DbModel>> GetModelsBatchAsync(int limit, int fetchSize)
        {
            var yielded = 0;
            var resultset = new List<DbModel>(fetchSize);

            do
            {
                await using(var cmd = new MySqlCommand($"SELECT Id, DayUtc, Val, MinVal, MaxVal FROM enumtest LIMIT {fetchSize};", _connection))
                {
                    await using(var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await reader.ReadAsync())
                        {
                            resultset.Add(new DbModel
                                          {
                                              Id = reader.GetString(0),
                                              DayUtc = reader.GetString(1),
                                              Val = reader.GetDouble(2),
                                              MinVal = reader.GetDouble(3),
                                              MaxVal = reader.GetDouble(4),
                                          });
                        }

                        yield return resultset;
                    }
                }

                yielded += fetchSize;
                resultset.Clear();
            } while (yielded < limit);
        }
    }
}
