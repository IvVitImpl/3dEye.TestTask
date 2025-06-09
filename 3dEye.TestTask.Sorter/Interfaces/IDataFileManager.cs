using _3dEye.TestTask.Sorter.Inplementation;

namespace _3dEye.TestTask.Sorter.Interfaces;

public interface IDataFileManager
{
    ChunkInfo[] ScanChunksInfo(string fileName, long maxBytes);
    string[] ReadDataLineByLine(string fileName, ChunkInfo chunk);
    void SaveData(string[] arr, string fileName);
    void MergeSortedChunks(string outputFileName, Action<long, long> onDataRead = null);
    void DeleteTmpFiles();
}