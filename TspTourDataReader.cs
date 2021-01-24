using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace SATSP
{
    public sealed class TspTourDataReader : IDisposable, IAsyncEnumerable<string>
    {
        private readonly StreamReader reader;
        public TspTourDataReader(string fileName)
        {
            reader = new StreamReader(fileName);
        }

        public void Dispose() => reader.Dispose();
        public async IAsyncEnumerator<string> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            var start = false;
            var end = false;
            while (!reader.EndOfStream)
            {
                var line = (await reader.ReadLineAsync())?.Trim();
                if (line is null) throw new InvalidDataException("Malformed TSP data.");
                if (!start)
                {
                    if (line.StartsWith("TOUR_SECTION"))
                    {
                        start = true;
                    }
                    continue;
                }

                if (line.StartsWith("-1") || line.StartsWith("EOF"))
                {
                    end = true;
                    continue;
                }

                if (!end)
                {
                    yield return line;
                }
            }
        }
    }
}
