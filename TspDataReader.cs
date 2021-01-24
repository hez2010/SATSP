using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace SATSP
{
    public sealed class TspDataReader : IDisposable, IAsyncEnumerable<Node>
    {
        private readonly StreamReader reader;
        public TspDataReader(string fileName)
        {
            reader = new StreamReader(fileName);
        }

        public void Dispose() => reader.Dispose();
        public async IAsyncEnumerator<Node> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            var start = false;
            var end = false;
            while (!reader.EndOfStream)
            {
                var line = (await reader.ReadLineAsync())?.Trim();
                if (line is null) throw new InvalidDataException("Malformed TSP data.");
                if (!start)
                {
                    if (line.StartsWith("NODE_COORD_SECTION"))
                    {
                        start = true;
                    }
                    continue;
                }

                if (line.StartsWith("EOF"))
                {
                    end = true;
                    continue;
                }

                if (!end)
                {
                    var data = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    if (data.Length != 3) throw new InvalidDataException("Malformed TSP data.");
                    yield return new Node(data[0], double.Parse(data[1]), double.Parse(data[2]));
                }
            }
        }
    }
}
