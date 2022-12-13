namespace Utilities;

public static class LockExtensions
{
    public static ReaderWriterLockSlimHelper EnterReadLock(ReaderWriterLockSlim guard, TimeSpan timeout)
    {
        bool EnterLockFunc(ReaderWriterLockSlim l) => l.TryEnterReadLock(timeout);
        void ExitLockAction(ReaderWriterLockSlim l)
        {
            if (l.IsReadLockHeld)
                l.ExitReadLock();
        }

        string lockOperation = nameof(EnterReadLock);

        return EnterLockGeneral(guard, EnterLockFunc, ExitLockAction, lockOperation);
    }
    public static ReaderWriterLockSlimHelper EnterWriteLock(ReaderWriterLockSlim guard, TimeSpan timeout)
    {
        Func<ReaderWriterLockSlim, bool> enterLockFunc = l => l.TryEnterWriteLock(timeout);
        Action<ReaderWriterLockSlim> exitLockAction = l =>
        {
            if (l.IsWriteLockHeld)
                l.ExitWriteLock();
        };
        string lockOperation = nameof(EnterWriteLock);

        return EnterLockGeneral(guard, enterLockFunc, exitLockAction, lockOperation);
    }
        

    private static ReaderWriterLockSlimHelper EnterLockGeneral(ReaderWriterLockSlim guard,
        Func<ReaderWriterLockSlim, bool> enterLockFunc, Action<ReaderWriterLockSlim> exitLockAction,
        string lockOperation)
    {
        bool lockTaken = false;
        try
        {
            lockTaken = enterLockFunc(guard);
            if (lockTaken)
            {
                return new ReaderWriterLockSlimHelper(guard, exitLockAction);
            }
            else
            {
                throw new TimeoutException(lockOperation);
            }
        }
        catch
        {
            if (lockTaken)
            {
                exitLockAction(guard);
            }

            throw;
        }
    }
}