namespace _3dEye.TestTask.Sorter.Inplementation;

public class StringsComparer : IComparer<string>
{
    public int Compare(string first, string second)
    {
        return Comparison(first, second);
    }
    public static int Comparison(string first, string second)
    {
        var indF = first.IndexOf('.');
        var indS = second.IndexOf('.');
        var strCompResult = first.AsSpan(indF + 1).CompareTo(second.AsSpan(indS + 1), StringComparison.OrdinalIgnoreCase);
        return strCompResult == 0 ? long.Parse(first.AsSpan(0, indF)).CompareTo(long.Parse(second.AsSpan(0, indS))) : strCompResult;
    }
}
