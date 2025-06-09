using _3dEye.TestTask.Generator.Implementation;
using _3dEye.TestTask.Generator.Interfaces;

var dictionaryPath = @".\Data\en_words_alpha.txt";
var maxThreads = 10;
var minSizeMb = 1;
var maxSizeMb = 102400;

try
{
    var sizeBytes = ConsoleHelper.AskFileSizeBytes(minSizeMb, maxSizeMb);
    var file = ConsoleHelper.AskDirName();

    IDictionaryLoader loader = new FileDictionaryLoader(dictionaryPath);
    var dictionary = loader.LoadDictionary();
    Console.WriteLine($"Dictionary loaded as {dictionary.Count} words from `{dictionaryPath}`");

    IDataGenerator generator = new DataGenerator(dictionary);
    IDataFileWriter writer = new DataFileWriter(generator, file.Dir, sizeBytes, maxThreads);
    writer.DataGenerated += ConsoleHelper.OnDataGeneratedPercent;

    var sw = System.Diagnostics.Stopwatch.StartNew();

    writer.GenerateAndSave(file.FileName);
    sw.Stop();
    Console.WriteLine($"{new string('\n', 6)}\rGeneration done in {sw.Elapsed}.\r\nPress Enter to exit.");

}
catch (Exception ex)
{
    Console.WriteLine(ex);
}


Console.ReadLine();
