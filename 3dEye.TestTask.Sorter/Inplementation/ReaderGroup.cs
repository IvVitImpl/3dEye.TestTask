namespace _3dEye.TestTask.Sorter.Inplementation;

public class ReaderGroup
{
    public int Index { get; set; }
    public int StartIndex { get; set; }
    public StreamReader[] Readers { get; set; }
    public int MinIndex { get; set; } = -1;
    public bool Finished { get; set; }
}
