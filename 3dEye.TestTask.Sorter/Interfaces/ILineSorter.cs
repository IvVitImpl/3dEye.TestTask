using _3dEye.TestTask.Sorter.Inplementation;

namespace _3dEye.TestTask.Sorter.Interfaces;

public interface ILineSorter
{
    string[] SortParallel(string[] arr, int parts);
}