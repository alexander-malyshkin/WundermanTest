namespace Utilities;

public class ReaderWriterLockSlimHelper : IDisposable
{
    private readonly ReaderWriterLockSlim _guard;
    private readonly Action<ReaderWriterLockSlim> _lockReleaseAction;

    internal ReaderWriterLockSlimHelper(ReaderWriterLockSlim guard,
        Action<ReaderWriterLockSlim> lockReleaseAction)
    {
        _guard = guard ?? throw new ArgumentNullException(nameof(guard));
        _lockReleaseAction = lockReleaseAction ?? throw new ArgumentNullException(nameof(lockReleaseAction));
    }
    public void Dispose()
    {
        _lockReleaseAction.Invoke(_guard);
    }
}