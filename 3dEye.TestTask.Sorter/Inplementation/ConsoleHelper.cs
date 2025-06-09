namespace _3dEye.TestTask.Sorter.Inplementation;

public static class ConsoleHelper
{
    public static void OnOperationDone(object sender, OperationDoneEventArgs e)
    {
        Console.Write(e.Message);
        Console.SetCursorPosition(0, Console.GetCursorPosition().Top);

        for (var i = 0; i < (int)e.OperatinImportance; i++)
            Console.WriteLine();
    }

    public static FileInfo AskFilePath()
    {
        while (true)
        {
            Console.WriteLine($"Please enter path to data file:");
            var pathStr = Console.ReadLine();
            if (ValidateFilePath(pathStr, out FileInfo fileInfo))
                return fileInfo;
            else
                Console.WriteLine("Incorrect input. Try again.");
        }
    }

    public static bool ValidateOutput(string path)
    {
        while (true)
        {
            if (File.Exists(path))
            {
                Console.WriteLine($"File {path} already exist. Enter 'Y' to overwrite the file or 'N' to close application");
                var inp = Console.ReadLine().ToUpper();
                if (inp == "Y")
                {
                    File.Delete(path);
                    return true;
                }
                if (inp == "N")
                    return false;
            }
            else
            {
                break;
            }
        }

        return true;
    }

    public static bool ValidateFilePath(string pathStr, out FileInfo path)
    {
        path = null;
        if (File.Exists(pathStr))
        {
            path = new FileInfo(pathStr);
            return true;
        }
        return false;
    }
}