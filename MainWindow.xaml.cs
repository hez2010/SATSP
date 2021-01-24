using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SATSP
{
    public class MainWindow : Window
    {
        private readonly MainViewModel viewModel = new();
        private readonly List<Node> data = new();
        private readonly List<string> tour = new();
        private double bestDis;

        public MainWindow()
        {
            InitializeComponent();
            viewModel.Status = "准备就绪";
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = viewModel;
        }

        private async void Load_Click(object sender, RoutedEventArgs args)
        {
            viewModel.Computing = true;
            try
            {
                data.Clear();
                tour.Clear();
                using var reader = new TspDataReader(viewModel.FileName);
                await foreach (var node in reader)
                {
                    data.Add(node);
                }

                if (!string.IsNullOrEmpty(viewModel.TourFileName))
                {
                    using var tourReader = new TspTourDataReader(viewModel.TourFileName);
                    await foreach (var node in tourReader)
                    {
                        tour.Add(node);
                    }
                    bestDis = tour.Select(i => data.First(j => j.Name == i)).GetTotalDistance();
                }
                else
                {
                    bestDis = -1;
                }

                viewModel.DataLoaded = true;
                viewModel.Status = $"TSP 数据已加载，城市数量：{data.Count}";
            }
            catch (Exception ex)
            {
                viewModel.Status = $"发生错误：{ex.Message}";
            }
            finally
            {
                viewModel.Computing = false;
            }
        }

        private string GetResultPath(Node[] result)
        {
            if (data.Count <= 0) return "";
            var names = result.Select(i => i.Name).ToArray();
            var index = names.IndexOf(data[0].Name);
            if (index == -1) return string.Join("-", names);
            return string.Join("-", names[index..].Concat(names[..index]));
        }

        private async void Solve_Click(object sender, RoutedEventArgs args)
        {
            viewModel.Computing = true;
            try
            {
                var stopWatch = new Stopwatch();
                viewModel.Result.Clear();
                viewModel.Status = "开始计算";
                TspSolver solver = viewModel.SimulatedAnnealing ?
                    new TspSolverSimulatedAnnealing(data.ToArray(), viewModel.Iteration, viewModel.TemperatureMax, viewModel.TemperatureMin, viewModel.Alpha, stopWatch)
                    : new TspSolverNormal(data.ToArray(), viewModel.Iteration, stopWatch);
                var iterCount = 0L;
                var progress = new Progress<ConcurrentQueue<ProgressModel>>();
                progress.ProgressChanged += (o, e) =>
                {
                    ProgressModel? lastModel = null;
                    viewModel.Result.BeginBulkOperation();
                    while (e.TryDequeue(out var model))
                    {
                        viewModel.Result.Add(new(model.X, model.Y));
                        lastModel = model;
                    }
                    viewModel.Result.EndBulkOperation();
                    if (lastModel != null)
                    {
                        if (viewModel.SimulatedAnnealing)
                        {
                            viewModel.Status = $"温度: {lastModel.Parameter}, 已迭代次数: {lastModel.X}, 当前计算最优解: {lastModel.Y}, 用时: {stopWatch.Elapsed.TotalSeconds}s";
                        }
                        else
                        {
                            viewModel.Status = $"进度: {lastModel.Parameter}%, 已迭代次数: {lastModel.X}, 当前计算最优解: {lastModel.Y}, 用时: {stopWatch.Elapsed.TotalSeconds}s";
                        }
                        iterCount = lastModel.X;
                        viewModel.ResultPath.Clear();
                        viewModel.ResultPath.BeginBulkOperation();
                        foreach (var i in lastModel.Path)
                        {
                            viewModel.ResultPath.Add(new(i.X, i.Y));
                        }
                        if (lastModel.Path.Length > 0) viewModel.ResultPath.Add(new(lastModel.Path[0].X, lastModel.Path[0].Y));
                        viewModel.ResultPath.EndBulkOperation();
                    }
                };
                stopWatch.Start();
                var result = await solver.Solve(progress);
                stopWatch.Stop();
                var dis = result.GetTotalDistance();
                viewModel.Status = $"计算最优解: {dis}{(bestDis >= 0 ? $", 参考最优解: {bestDis}, 相差比率: {(dis - bestDis) * 100 / bestDis}%" : "")}, " +
                    $"用时: {stopWatch.Elapsed.TotalSeconds}s, " +
                    $"迭代次数: {iterCount}, " +
                    $"计算最优路径: {GetResultPath(result)}" +
                    $"{((bestDis >= 0) ? $", 参考最优路径: {string.Join("-", tour)}" : "")}";
            }
            catch (Exception ex)
            {
                viewModel.Status = $"发生错误: {ex.Message}";
            }
            finally
            {
                viewModel.Computing = false;
            }
        }
    }
}