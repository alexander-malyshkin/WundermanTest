using Model;

namespace Infrastructure;

public sealed class FileProcessor : IFileProcessor
{
    public async Task ProcessFile(string filePath, Action<IEnumerable<string>> completionCallback, CancellationToken ct)
    {
        if (completionCallback == null) throw new ArgumentNullException(nameof(completionCallback));
        
        // processing file code would be here
        
        // here we mimick some work delay
        await Task.Delay(500, ct);

        var results = new[] { "test1", "test2" };
        
        completionCallback.Invoke(results);
    }
}