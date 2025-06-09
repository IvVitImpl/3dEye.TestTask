using _3dEye.TestTask.Generator.Interfaces;

namespace _3dEye.TestTask.Generator.Implementation;

public class DataFileWriter : IDataFileWriter
{
    private readonly Lock _lock = new Lock();
    private readonly int _bufferSizeBytes = 102400;

    private readonly IDataGenerator _dataGenerator;
    private readonly string _outputDir;
    private readonly int _threadsCount;
    private readonly long _fileSizeBytes;

    public DataFileWriter(IDataGenerator dataGenerator, string outputDir, long fileSizeBytes, int threadsCount)
    {
        _dataGenerator = dataGenerator;
        _outputDir = outputDir;
        _fileSizeBytes = fileSizeBytes;
        _threadsCount = threadsCount;
    }

    public void GenerateAndSave(string fileName)
    {
        var writerOptions = new FileStreamOptions() { Access = FileAccess.ReadWrite, Mode = FileMode.Create, Share = FileShare.ReadWrite };

        using (var writer = new StreamWriter(_outputDir + fileName, writerOptions))
        {
            var currentFileSize = 0L;

            Parallel.For(0, _threadsCount, (i) =>
            {
                Span<char> buffer = stackalloc char[_bufferSizeBytes];

                while (true)
                {
                    if (currentFileSize >= _fileSizeBytes)
                        break;

                    int charsInBuffer = _dataGenerator.FillBuffer(buffer);

                    lock (_lock)
                    {
                        if (currentFileSize >= _fileSizeBytes)
                            break;

                        charsInBuffer = FitLastRows(buffer, currentFileSize, charsInBuffer);

                        if(charsInBuffer < 0)
                            break;

                        writer.Write(buffer[0..charsInBuffer]);

                        currentFileSize += charsInBuffer;
                        charsInBuffer = 0;

                        OnDataGenerated(currentFileSize);
                    }
                }
            });
        }
    }

    private int FitLastRows(Span<char> buffer, long currentFileSize, int charsInBuffer)
    {
        if (currentFileSize + charsInBuffer > _fileSizeBytes)
        {
            var lastChunk = buffer.Slice(0, (int)(_fileSizeBytes - currentFileSize));
            var endLineIndex = lastChunk.LastIndexOf(Environment.NewLine);
            if (endLineIndex >= 0)
                return buffer.Slice(0, (int)(_fileSizeBytes - currentFileSize)).LastIndexOf(Environment.NewLine)+2;
            else
                return -1;
        }

        return charsInBuffer;
    }

    private int _lastPercent; 
    private void OnDataGenerated(long currentFileSize)
    {
        var percent = (int)Math.Round(currentFileSize / ((double)_fileSizeBytes / 100), 0);
        if (percent != _lastPercent) 
        {
            DataGenerated?.Invoke(this, new DataGeneratedEventArgs() { PercentGenerated =  percent});
            _lastPercent = percent;
        }
    }

    public event EventHandler<DataGeneratedEventArgs> DataGenerated;
}
