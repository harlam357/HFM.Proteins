using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace HFM.Proteins.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<TabDelimitedTextSerializerBenchmark>(
            ManualConfig.Create(DefaultConfig.Instance)
                .WithOptions(ConfigOptions.DisableOptimizationsValidator));
    }
}
