using _3dEye.TestTask.Sorter.Interfaces;

namespace _3dEye.TestTask.Sorter.Inplementation;

public class DataFileManager : IDataFileManager
{
    private readonly string _dataDir;
    public static string ChunkFileExtension = ".chunk";
    public DataFileManager(string dataDir)
    {
        _dataDir = dataDir;
    }

    public ChunkInfo[] ScanChunksInfo(string fileName, long maxBytes)
    {
        using (var reader = new StreamReader(_dataDir + fileName, new FileStreamOptions() { Access = FileAccess.Read, Mode = FileMode.Open, Share = FileShare.ReadWrite }))
        {
            var chunks = reader.BaseStream.Length / maxBytes + 1;
            var bytesInChunk = reader.BaseStream.Length / chunks;

            var result = new ChunkInfo[chunks];

            Span<char> buff = stackalloc char[4096];

            var chunkCount = 0;

            for (var i = 0; i < chunks; i++)
            {
                var bytesRead = 0L;
                var rowsCount = 0;

                while (true)
                {
                    var read = reader.Read(buff);
                    bytesRead += read;
                    rowsCount += buff[..read].Count('\n');

                    if ((bytesRead > bytesInChunk && i != chunks - 1) || reader.EndOfStream)
                    {
                        var lastEndLineInd = buff[..read].LastIndexOf('\n');
                        result[chunkCount] = new ChunkInfo()
                        {
                            Length = bytesRead - read + lastEndLineInd + 1,
                            Offset = result.Sum(x => x?.Length ?? 0),
                            Rows = rowsCount,
                        };

                        reader.BaseStream.Position = result.Sum(x => x?.Length ?? 0);
                        reader.DiscardBufferedData();

                        chunkCount++;
                        break;
                    }
                }
            }

            return result;
        }
    }

    public string[] ReadDataLineByLine(string fileName, ChunkInfo chunk)
    {
        var arr = new string[chunk.Rows];

        using (var reader = new StreamReader(_dataDir + fileName, new FileStreamOptions() { Access = FileAccess.Read, Mode = FileMode.Open, Share = FileShare.ReadWrite }))
        {
            var bytsRead = 0L;
            var rowIndex = 0;
            reader.BaseStream.Position = chunk.Offset;
            while (bytsRead < chunk.Length)
            {
                var line = reader.ReadLine();
                arr[rowIndex++] = line;
                bytsRead += line.Length + 2;
            }
        }
        return arr;
    }

    public void SaveData(string[] arr, string fileName)
    {
        var writerOptions = new FileStreamOptions() { Access = FileAccess.Write, Mode = FileMode.Create, Share = FileShare.ReadWrite };
        using (var writer = new StreamWriter(_dataDir + fileName + ChunkFileExtension, writerOptions))
        {
            foreach (var row in arr)
                writer.WriteLine(row);
        }
    }

    public void MergeSortedChunks(string outputFileName, Action<long, long> onDataRead = null)
    {
        var writerOptions = new FileStreamOptions() { Access = FileAccess.Write, Mode = FileMode.Create, Share = FileShare.ReadWrite };
        using (var writer = new StreamWriter(_dataDir + outputFileName, writerOptions))
        {
            using (var reader = new SortedChunksReader(_dataDir, ChunkFileExtension))
            {
                var linesRead = 0L;
                while (true)
                {
                    var readSucces = reader.TryGetMinimalLine(out var line);
                    if (readSucces)
                    {
                        linesRead++;
                        writer.WriteLine(line);
                        if (onDataRead != null && (linesRead % 1_000_000 == 0 || reader.BytesRead == reader.TotalBytes))
                            onDataRead(reader.BytesRead, reader.TotalBytes);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    public void DeleteTmpFiles()
    {
        var files = Directory.GetFiles(_dataDir, $"*{ChunkFileExtension}");
        foreach (var file in files)
            new FileInfo(file).Delete();
    }

}
