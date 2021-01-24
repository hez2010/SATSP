using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SATSP
{
    public class TspSolverSimulatedAnnealing : TspSolver
    {
        private readonly static Random random = new();
        private readonly double temperatureMax;
        private readonly double temperatureMin;
        private readonly double alpha;

        public TspSolverSimulatedAnnealing(Node[] data, int iteration, double temperatureMax, double temperatureMin, double alpha, Stopwatch stopwatch) : base(data, iteration, stopwatch)
        {
            this.temperatureMax = temperatureMax;
            this.temperatureMin = temperatureMin;
            this.alpha = alpha;
        }

        public override Task<Node[]> Solve(IProgress<ConcurrentQueue<ProgressModel>> progress) =>
            Task.Run(() =>
            {
                var sampler = new IntervalSampler(200);
                var current = data.Shuffle().ToArray();
                var reportQueue = new ConcurrentQueue<ProgressModel>();
                reportQueue.Enqueue(new(0, 0, current.GetTotalDistance(), current));
                progress.Report(reportQueue);
                var count = 0L;
                var bestDis = current.GetTotalDistance();
                Node[] newNodes;
                var bestNodes = current;
                var temperature = temperatureMax;
                while (temperature >= temperatureMin)
                {
                    var i = 0;
                    while (i < iteration)
                    {
                        i++;
                        count++;
                        newNodes = Utils.Transform(current);
                        var dis = current.GetTotalDistance();
                        var newDis = newNodes.GetTotalDistance();
                        if (newDis <= dis)
                        {
                            current = newNodes;
                            if (newDis < bestDis)
                            {
                                bestDis = newDis;
                                bestNodes = current;
                            }
                        }
                        else
                        {
                            if (Math.Exp(-(newDis - dis) / temperature) > random.NextDouble())
                            {
                                current = newNodes;
                            }
                        }
                    }
                    temperature *= alpha;
                    reportQueue.Enqueue(new(temperature, count, bestNodes.GetTotalDistance(), bestNodes));
                    if (sampler.CheckNow())
                    {
                        stopwatch.Stop();
                        progress.Report(reportQueue);
                        stopwatch.Start();
                    }
                }
                progress.Report(reportQueue);
                return bestNodes;
            });
    }
}
