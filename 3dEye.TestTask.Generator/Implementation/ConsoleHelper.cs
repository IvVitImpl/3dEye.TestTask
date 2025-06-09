namespace _3dEye.TestTask.Generator.Implementation;
public static class ConsoleHelper
{
    public static (string Dir, string FileName) AskDirName()
    {
        while (true)
        {
            Console.WriteLine("Enter output file fullpath:");
            var filePath = Console.ReadLine();
            var dir = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);
            if (Directory.Exists(dir) && !string.IsNullOrWhiteSpace(fileName))
            {
                return new(dir + Path.DirectorySeparatorChar, fileName);
            }
            else
                Console.WriteLine("Wrong path. Try again.");
        }
    }

    public static void OnDataGeneratedPercent(object sender, DataGeneratedEventArgs e)
    {
        Console.WriteLine($"Generated:{e.PercentGenerated} %   ");
        Console.WriteLine($"Memory usage: {GC.GetGCMemoryInfo().TotalCommittedBytes:### ### ### ###} B   ");
        Console.WriteLine($"GC 0 count:{GC.CollectionCount(0)}   ");
        Console.WriteLine($"GC 1 count:{GC.CollectionCount(1)}   ");
        Console.WriteLine($"GC 2 count:{GC.CollectionCount(2)}   ");
        Console.SetCursorPosition(0, Console.CursorTop - 5);
    }

    public static long AskFileSizeBytes(int minSizeMb, int maxSizeMb)
    {
        while (true)
        {
            Console.WriteLine($"Enter file size in MB (max {maxSizeMb} MB):");
            var sizeMbStr = Console.ReadLine();
            if (long.TryParse(sizeMbStr, out var sizeMb) && sizeMb >= minSizeMb && sizeMb <= maxSizeMb)
            {
                return sizeMb * 1024 * 1024;
            }
            else
                Console.WriteLine("Incorrect input. Try again.");
        }
    }
}