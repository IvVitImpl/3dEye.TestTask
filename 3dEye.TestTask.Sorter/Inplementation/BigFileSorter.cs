using _3dEye.TestTask.Sorter.Interfaces;
using System.Diagnostics;

namespace _3dEye.TestTask.Sorter.Inplementation;

public class BigFileSorter : IBigFileSorter
{
    private readonly IDataFileManager _dataFileManager;
    private readonly ILineSorter _sorter;
    private readonly long filePartMaxSize =  1024 * 1024 * 1024 * 1L;
    private readonly int parallelSortParts = 12;
    public BigFileSorter(IDataFileManager dataFileManager, ILineSorter sorter)
    {
        _dataFileManager = dataFileManager;
        _sorter = sorter;
    }

    private Stopwatch _stopwatch;
    private TimeSpan _timeTotal;
    public void SortFile(string fileName, string sortedFileName)
    {
        _stopwatch = Stopwatch.StartNew();
        _timeTotal = new TimeSpan();
        
        var chunksInfo = _dataFileManager.ScanChunksInfo(fileName, filePartMaxSize);
        OnOperationDone("Scan DONE in {0}", OperatinImportance.Medium);

        for (int chunkIndex = 0; chunkIndex < chunksInfo.Length; chunkIndex++)
        {
            ProcessChunk(fileName, chunksInfo, chunkIndex);
        }

        OnOperationDone($"SPlitting DONE in {_timeTotal}", OperatinImportance.Medium);

        _dataFileManager.MergeSortedChunks(sortedFileName, OnRead);
        OnOperationDone("Chunks merge DONE in {0}", OperatinImportance.Medium);

        //_dataFileManager.DeleteTmpFiles();
        OnOperationDone("Delete temp files: DONE in {0}", OperatinImportance.Medium);

        OnOperationDone($"All DONE in {_timeTotal}",  OperatinImportance.High);
        _stopwatch.Stop();
    }


    private void ProcessChunk(string fileName,  ChunkInfo[] chunksInfo, int chunkIndex)
    {
        var arr = _dataFileManager.ReadDataLineByLine(fileName, chunksInfo[chunkIndex]);
        OnOperationDone($"Part {chunkIndex} read DONE in {{0}}");

       var parts = _sorter.SortParallel(arr, parallelSortParts);
        OnOperationDone($"Part {chunkIndex} sort DONE in {{0}}");

        _dataFileManager.SaveData(parts, $"sorted_{chunkIndex}");
        OnOperationDone($"Part {chunkIndex} write DONE in {{0}}", OperatinImportance.Medium);
    }

    private void OnRead(long read, long total)
    {
        OperationDone?.Invoke(this, new OperationDoneEventArgs() { Message = $"{read:### ### ### ###} of {total:### ### ### ###} bytes merged", 
            OperatinImportance = read == total ? OperatinImportance.Medium : OperatinImportance.Smallest });
    }

    private void OnOperationDone(string msgFormat, OperatinImportance importance = OperatinImportance.Small)
    {
        OperationDone?.Invoke(this, new OperationDoneEventArgs() { Message = string.Format(msgFormat, _stopwatch.Elapsed), OperatinImportance = importance });
        _timeTotal += _stopwatch.Elapsed;
        _stopwatch.Restart();
    }

    public event EventHandler<OperationDoneEventArgs> OperationDone;
}
