using _3dEye.TestTask.Generator.Implementation;

namespace _3dEye.TestTask.Generator.Interfaces;

public interface IDataFileWriter
{
    event EventHandler<DataGeneratedEventArgs> DataGenerated;
    void GenerateAndSave(string fileName);
}
