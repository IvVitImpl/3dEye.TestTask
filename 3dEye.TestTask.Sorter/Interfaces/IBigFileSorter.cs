using _3dEye.TestTask.Sorter.Inplementation;

namespace _3dEye.TestTask.Sorter.Interfaces;

public interface IBigFileSorter
{
    event EventHandler<OperationDoneEventArgs> OperationDone;
    void SortFile(string fileName, string sortedFileName);
}