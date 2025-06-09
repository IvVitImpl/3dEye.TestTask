using _3dEye.TestTask.Sorter.Inplementation;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

public static class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<SortingBenchmark>();
    }
}


[MemoryDiagnoser]
public class SortingBenchmark
{
    private char[] _rawData;

    [Params(1000, 10000, 70000, 136000)]
    public int LinesCount;


    [GlobalSetup]
    public void Setup()
    {
        using (var reader = new StreamReader(@".\Data\5_mb_136068_lines.txt", new FileStreamOptions() { Access = FileAccess.Read, Mode = FileMode.Open, Share = FileShare.ReadWrite }))
        {
            _rawData = new char[reader.BaseStream.Length];
            reader.Read(_rawData);
        }
    }

    [Benchmark(Baseline = true)]
    public void ReadAndSortAsStrings() 
    {
        var lines = _rawData.AsSpan().EnumerateLines();
        var buff = new string[LinesCount];

        for (int i = 0; i < buff.Length; i++)
        {
            lines.MoveNext();
            buff[i] = new string(lines.Current);
        }

        Array.Sort(buff, StringsComparer.Comparison);
    }

    [Benchmark]
    public void ReadAndSortAsLines()
    {
        var lines = _rawData.AsSpan().EnumerateLines();
        var buff = new Line[LinesCount];

        for (int i = 0; i < buff.Length; i++)
        {
            lines.MoveNext();
            buff[i] = new Line(lines.Current);
        }

        Array.Sort(buff, LinesComparer.Comparison);
    }
}


