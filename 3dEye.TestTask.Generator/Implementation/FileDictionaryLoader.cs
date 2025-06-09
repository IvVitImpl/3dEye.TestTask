using _3dEye.TestTask.Generator.Interfaces;

namespace _3dEye.TestTask.Generator.Implementation;

public class FileDictionaryLoader : IDictionaryLoader
{
    private readonly string _path;

    public FileDictionaryLoader(string path)
    {
        _path = path;
    }

    public List<string> LoadDictionary()
    {
        var result = new List<string>();

        using (var reader = new StreamReader(_path, new FileStreamOptions() { Access = FileAccess.Read, Mode = FileMode.Open, Share = FileShare.ReadWrite }))
        {
            while (reader.Peek() >= 0)
                result.Add(reader.ReadLine());
        }

        return result;
    }
}