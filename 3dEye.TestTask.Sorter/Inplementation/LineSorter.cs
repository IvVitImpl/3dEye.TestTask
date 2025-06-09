using _3dEye.TestTask.Sorter.Interfaces;
using System.Collections.Concurrent;

namespace _3dEye.TestTask.Sorter.Inplementation;
public class LineSorter : ILineSorter
{
    public string[] SortParallel(string[] arr, int parts)
    {
        var chunks = Enumerable.Chunk(arr, (int)Math.Ceiling((double)arr.Length / parts)).ToArray();
        Parallel.ForEach(chunks, (chunk) =>
        {
            Array.Sort(chunk, StringsComparer.Comparison);
        });

        return MergeParallel(chunks);
    }

    private string[] MergeParallel(string[][] chunks)
    {
        var groups = Enumerable.Chunk(chunks, 2);
        var result = new ConcurrentBag<string[]>();

        Parallel.ForEach(groups, (group) =>
        {
            var merged = MergeChunks(group);
            result.Add(merged);
        });
        
        if (result.Count == 1)
            return result.Take(1).First();
        else
            return MergeParallel(result.ToArray());
    }

    private string[] MergeChunks(string[][] chunks)
    {
        if(chunks.Length == 1)
            return chunks[0];

        var indexes = new int[chunks.Length];
        var result = new string[chunks.Sum(c => c.Length)];

        for (int resultCounter = 0; resultCounter < result.Length; resultCounter++)
        {
            var smallestInd = 0;

            for (int chunkCounter = 0; chunkCounter < chunks.Length; chunkCounter++)
            {
                if (indexes[chunkCounter] >= chunks[chunkCounter].Length)
                    continue;

                var current = chunks[chunkCounter][indexes[chunkCounter]];
                if (result[resultCounter] == null || StringsComparer.Comparison(current, result[resultCounter]) < 0)
                {
                    result[resultCounter] = current;
                    smallestInd = chunkCounter;
                }
            }

            indexes[smallestInd]++;
        }

        return result;
    }

}
