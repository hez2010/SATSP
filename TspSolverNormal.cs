using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SATSP
{
    public class TspSolverNormal : TspSolver
    {
        public TspSolverNormal(Node[] data, int iteration, Stopwatch stopwatch) : base(data, iteration, stopwatch) { }

        public override Task<Node[]> Solve(IProgress<ConcurrentQueue<ProgressModel>> progress) =>
            Task.Run(() =>
            {
                var sampler = new IntervalSampler(200);
                var current = data.Shuffle().ToArray();
                var reportQueue = new ConcurrentQueue<ProgressModel>();
                reportQueue.Enqueue(new(0, 0, current.GetTotalDistance(), current));
                progress.Report(reportQueue);
                var i = 0;
                Node[] newNodes;
                while (i < iteration)
                {
                    i++;
                    newNodes = Utils.Transform(current);
                    var dis = current.GetTotalDistance();
                    var newDis = newNodes.GetTotalDistance();
                    if (newDis <= dis)
                    {
                        current = newNodes;
                        reportQueue.Enqueue(new(i * 100.0 / iteration, i, newDis, current));
                    }
                    else
                    {
                        reportQueue.Enqueue(new(i * 100.0 / iteration, i, dis, current));
                    }
                    if (sampler.CheckNow())
                    {
                        stopwatch.Stop();
                        progress.Report(reportQueue);
                        stopwatch.Start();
                    }
                }
                progress.Report(reportQueue);
                return current;
            });

    }
}
