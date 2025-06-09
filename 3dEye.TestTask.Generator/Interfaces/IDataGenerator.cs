namespace _3dEye.TestTask.Generator.Interfaces;

public interface IDataGenerator
{
    int FillBuffer(Span<char> buffer);
}

