namespace _3dEye.TestTask.Sorter.Inplementation;

public class Line
{
    public Line(ReadOnlySpan<char> line)
    {
        var ind = line.IndexOf('.');
        Id = long.Parse(line.Slice(0, ind));
        Data = new string(line.Slice(ind + 1));
    }

    public long Id { get; set; }
    public string Data { get; set; }
}
