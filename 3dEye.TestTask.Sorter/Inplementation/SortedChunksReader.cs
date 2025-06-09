namespace _3dEye.TestTask.Sorter.Inplementation;

public class SortedChunksReader : IDisposable
{
    private long _bytesTotal;
    private long _bytesRead;
    private ReaderGroup[] _groups;
    private string[] _linesCache;
    public long TotalBytes => _bytesTotal;
    public long BytesRead => _bytesRead;

    public SortedChunksReader(string dataDir, string filesExtension)
    {
        Init(Directory.GetFiles(dataDir, $"*{filesExtension}"));
    }

    private void Init(string[] files)
    {
        if (files.Length == 0)
            throw new FileNotFoundException();

        var readerOptions = new FileStreamOptions() { Access = FileAccess.Read, Mode = FileMode.Open, Share = FileShare.ReadWrite };
        var readers = files.Select(f => new StreamReader(f, readerOptions)).ToArray();

        _bytesTotal = readers.Sum(r => r.BaseStream.Length);
        _linesCache = new string[readers.Length];
        _groups = SplitReaders(readers);
    }

    public bool TryGetMinimalLine(out string minLine)
    {
        foreach (var group in _groups)
            GetMinInGroup(group);

        minLine = null;
        var minGroup = (ReaderGroup)null;

        for (int i = 0; i < _groups.Length; i++)
        {
            if (_groups[i].Finished)
                continue;

            if (minGroup == null || StringsComparer.Comparison(_linesCache[_groups[i].MinIndex], _linesCache[minGroup.MinIndex]) < 0)
                minGroup = _groups[i];
        }

        if (minGroup != null)
        {
            minLine = _linesCache[minGroup.MinIndex];
            _bytesRead += minLine.Length + Environment.NewLine.Length;
            _linesCache[minGroup.MinIndex] = null;
            minGroup.MinIndex = -1;
        }

        return minGroup != null;
    }

    private void GetMinInGroup(ReaderGroup data)
    {
        if (data.MinIndex < 0 && !data.Finished)
        {
            for (int i = 0; i < data.Readers.Length; i++)
            {
                var cacheInd = data.StartIndex + i;

                if (_linesCache[cacheInd] == null && !data.Readers[i].EndOfStream)
                    _linesCache[cacheInd] = data.Readers[i].ReadLine();

                if (_linesCache[cacheInd] == null)
                    continue;

                if (data.MinIndex < 0 || StringsComparer.Comparison(_linesCache[cacheInd], _linesCache[data.MinIndex]) < 0)
                    data.MinIndex = cacheInd;
            }

            if (data.MinIndex < 0)
                data.Finished = true;
        }
    }

    private ReaderGroup[] SplitReaders(StreamReader[] readers)
    {
        var readerPartsCount = (int)Math.Sqrt(readers.Length);
        var groups = Enumerable.Chunk(readers, readers.Length / readerPartsCount).ToArray();
        return groups.Select((g, i) =>
        {
            var startInd = groups.Take(i).Sum(r => r.Length);
            return new ReaderGroup() { StartIndex = startInd, Readers = g, Index = i };
        })
        .ToArray();
    }

    public void Dispose()
    {
        foreach (var group in _groups)
            foreach (var reader in group.Readers)
                reader.Dispose();
    }
}