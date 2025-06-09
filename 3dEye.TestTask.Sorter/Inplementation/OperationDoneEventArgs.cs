namespace _3dEye.TestTask.Sorter.Inplementation;

public class OperationDoneEventArgs : EventArgs
{
    public string Message { get; set; }
    public OperatinImportance OperatinImportance { get; set; }
}
