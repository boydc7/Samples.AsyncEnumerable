using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Samples.AsyncEnumerable
{
    internal class Program
    {
        private const int _limit = 250000;
        private const int _fetchSize = 1000;

        private static async Task Main()
        {
            var dbContext = new DbContext("Server=<SERVER>;Database=<DBNAME>;Uid=<USERID>;Pwd=<PWDHERE>;default command timeout=1800;ConnectionReset=True;Convert Zero Datetime=true;AllowUserVariables=True;");

            Console.WriteLine("------------------------------------------------------------------------------");
            Console.WriteLine("Starting DoSync");
            Console.WriteLine("------------------------------------------------------------------------------");
            DoSync(dbContext);

            Console.WriteLine("");
            Console.WriteLine("------------------------------------------------------------------------------");
            Console.WriteLine("Starting DoAsync");
            Console.WriteLine("------------------------------------------------------------------------------");
            await DoAsync(dbContext).ConfigureAwait(false);

            Console.WriteLine("");
            Console.WriteLine("------------------------------------------------------------------------------");
            Console.WriteLine("Starting DoBatchAsync");
            Console.WriteLine("------------------------------------------------------------------------------");
            await DoBatchAsync(dbContext).ConfigureAwait(false);

            Console.WriteLine("");
            Console.WriteLine("------------------------------------------------------------------------------");
            Console.WriteLine("Done - press a key to continue");
            Console.WriteLine("------------------------------------------------------------------------------");

            Console.ReadKey();
        }

        private static void DoSync(DbContext dbContext)
        {
            var models = dbContext.GetModels(200, 200).ToList();

            var sw = Stopwatch.StartNew();

            var count = 0;

            foreach (var dbModel in dbContext.GetModels(_limit, _fetchSize))
            {
                count++;
            }

            sw.Stop();

            Console.WriteLine($"DoSync Got [{count}] models in [{sw.ElapsedMilliseconds}] milliseconds");
        }

        private static async Task DoAsync(DbContext dbContext)
        {
            var count = 0;

            await foreach (var wumodel in dbContext.GetModelsAsync(200, 200).ConfigureAwait(false))
            {
                count++;
            }

            count = 0;

            var sw = Stopwatch.StartNew();

            await foreach (var model in dbContext.GetModelsAsync(_limit, _fetchSize).ConfigureAwait(false))
            {
                count++;
            }

            sw.Stop();

            Console.WriteLine($"DoAsync Got [{count}] models in [{sw.ElapsedMilliseconds}] milliseconds");
        }

        private static async Task DoBatchAsync(DbContext dbContext)
        {
            var count = 0;

            await foreach (var wumodel in dbContext.GetModelsBatchAsync(200, 200).ConfigureAwait(false))
            {
                count++;
            }

            count = 0;

            var sw = Stopwatch.StartNew();

            await foreach (var modelBatch in dbContext.GetModelsBatchAsync(_limit, _fetchSize).ConfigureAwait(false))
            {
                foreach (var model in modelBatch)
                {
                    count++;
                }
            }

            sw.Stop();

            Console.WriteLine($"DoBatchAsync Got [{count}] models in [{sw.ElapsedMilliseconds}] milliseconds");
        }
    }
}
