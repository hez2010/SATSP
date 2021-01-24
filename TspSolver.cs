using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SATSP
{
    public abstract class TspSolver
    {
        protected readonly Node[] data;
        protected readonly int iteration;
        protected readonly Stopwatch stopwatch;

        public TspSolver(Node[] data, int iteration, Stopwatch stopwatch)
        {
            this.data = data;
            this.iteration = iteration;
            this.stopwatch = stopwatch;
        }

        public abstract Task<Node[]> Solve(IProgress<ConcurrentQueue<ProgressModel>> progress);
    }
}
