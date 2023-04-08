using BenchmarkDotNet.Attributes;

namespace HFM.Proteins.Benchmarks;

[MemoryDiagnoser]
public class TabDelimitedTextSerializerBenchmark
{
    private const int Cycles = 10;

    private static readonly TabDelimitedTextSerializer _Serializer = new();
    private byte[]? _bytes;

    [GlobalSetup]
    public void GlobalSetup() => _bytes = File.ReadAllBytes("TestFiles\\ProjectInfo.tab");

    [Benchmark]
    public void TabDelimitedTextSerializer_Deserialize()
    {
        for (int i = 0; i < Cycles; i++)
        {
            using var stream = new MemoryStream(_bytes!);
            _ = _Serializer.Deserialize(stream);
        }
    }

    [Benchmark]
    public async Task TabDelimitedTextSerializer_DeserializeAsync()
    {
        for (int i = 0; i < Cycles; i++)
        {
            await using var stream = new MemoryStream(_bytes!);
            _ = await _Serializer.DeserializeAsync(stream);
        }
    }
}
