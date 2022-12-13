namespace Model;

public interface IFileProcessor
{
    Task ProcessFile(string filePath, Action<IEnumerable<string>> completionCallback, CancellationToken ct);
}