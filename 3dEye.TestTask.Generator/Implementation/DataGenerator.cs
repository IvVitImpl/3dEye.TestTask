using _3dEye.TestTask.Generator.Interfaces;

namespace _3dEye.TestTask.Generator.Implementation;

public class DataGenerator : IDataGenerator
{
    private readonly List<string> _dictionary;
    private readonly char _idSeparator = '.';
    private readonly char _wordSeparator = ' ';
    private readonly int _maxWordsInRow = 5;
    private long _rowId = 0;

    public DataGenerator(List<string> dictionary)
    {
        _dictionary = dictionary;
    }

    public int FillBuffer(Span<char> buffer)
    {
        int charsInBuffer = 0;
        while (TryAppendLIne(buffer.Slice(charsInBuffer), Interlocked.Increment(ref _rowId), out int charsAdded))
        {
            charsInBuffer += charsAdded;
        }
        return charsInBuffer;
    }

    private bool TryAppendId(Span<char> buffer, long id, ref int charsAdded)
    {
        Span<char> temp = stackalloc char[11];
        
        if (id.TryFormat(temp, out int size) && charsAdded + size <= buffer.Length)
        {
            temp[0..size].CopyTo(buffer.Slice(charsAdded));
            charsAdded += size;
            return true;
        }

        return false;
    }

    private bool TryAppendChar(Span<char> buffer, char chr, ref int charsAdded)
    {
        if (charsAdded >= buffer.Length)
            return false;
        
        buffer[charsAdded] = chr;
        charsAdded++;

        return true;
    }

    private bool TryAppendString(Span<char> buffer, string str, ref int charsAdded)
    {
        if (!str.TryCopyTo(buffer.Slice(charsAdded)))
            return false;

        charsAdded += str.Length;

        return true;
    }

    private bool TryAppendLIne(Span<char> buffer, long id, out int lineSize)
    {
        lineSize = 0; 
        var charsAdded = 0;

        if (!TryAppendId(buffer, id, ref charsAdded))
            return false;

        if (!TryAppendChar(buffer, _idSeparator, ref charsAdded))
            return false;

        var wordsInRow = Random.Shared.Next(1, _maxWordsInRow + 1);
        
        for (var i = 1; i <= wordsInRow; i++)
        {
            var wordIndex = Random.Shared.Next(_dictionary.Count);
            
            if (!TryAppendString(buffer, _dictionary[wordIndex], ref charsAdded))
                return false;

            if (i != wordsInRow)
            {
                if (!TryAppendChar(buffer, _wordSeparator, ref charsAdded))
                    return false;
            }
            else
            {
                if (!TryAppendString(buffer, Environment.NewLine, ref charsAdded))
                    return false;
            }
        }

        lineSize = charsAdded;

        return true;
    }
}
