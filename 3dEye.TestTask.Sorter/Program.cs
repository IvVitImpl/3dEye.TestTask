using _3dEye.TestTask.Sorter.Inplementation;
using _3dEye.TestTask.Sorter.Interfaces;


if (!ConsoleHelper.ValidateFilePath(args.Length > 0 ? args[0] : "", out var file))
    file = ConsoleHelper.AskFilePath();

var outputFile = $"sorted_{file.Name}";
if (!ConsoleHelper.ValidateOutput(Path.Combine(file.Directory.FullName, outputFile)))
    return;

try
{
	IDataFileManager dataFileManager = new DataFileManager(file.Directory.FullName + Path.DirectorySeparatorChar);
	ILineSorter lineSorter = new LineSorter();
	IBigFileSorter fileSorter = new BigFileSorter(dataFileManager, lineSorter);
	fileSorter.OperationDone += ConsoleHelper.OnOperationDone;

	Console.WriteLine($"Sorting started. File: {file.FullName}");
	fileSorter.SortFile(file.Name, outputFile);

	Console.WriteLine("Press Enter to exit.");

}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

Console.ReadLine();