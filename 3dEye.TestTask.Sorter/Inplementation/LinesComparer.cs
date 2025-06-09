namespace _3dEye.TestTask.Sorter.Inplementation;

public class LinesComparer : IComparer<Line>
{
    public int Compare(Line first, Line second)
    {
        return Comparison(first, second);
    }
    public static int Comparison(Line first, Line second)
    {
        var strCompResult = string.CompareOrdinal(first.Data, second.Data);
        return strCompResult == 0 ? first.Id.CompareTo(second.Id) : strCompResult;
    }
}