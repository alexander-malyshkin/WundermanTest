using Model;
using Model.Exceptions;
using Utilities;

namespace Infrastructure;

public sealed class DataProcessorService : IDataProcessorService
{
    private readonly IDictionary<Guid, DataJobDTO> _jobsDictionary = new Dictionary<Guid, DataJobDTO>();
    private readonly IFileProcessor _fileProcessor;

    private readonly IDictionary<DataJobStatus, List<DataJobDTO>> _jobsByStatus =
        new Dictionary<DataJobStatus, List<DataJobDTO>>();
    
    private readonly ReaderWriterLockSlim _readerWriterLock = new(LockRecursionPolicy.NoRecursion);
    private static readonly TimeSpan EnterWriteLockTimeout = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan EnterReadLockTimeout = TimeSpan.FromSeconds(30);

    public DataProcessorService(IFileProcessor fileProcessor)
    {
        _fileProcessor = fileProcessor;
    }

    public IEnumerable<DataJobDTO> GetAllDataJobs()
    {
        using (var releaser = LockExtensions.EnterReadLock(_readerWriterLock,
                   EnterReadLockTimeout))
        {
            return _jobsDictionary.Select(_ => _.Value).ToArray();
        }
    }

    public IEnumerable<DataJobDTO> GetDataJobsByStatus(DataJobStatus status)
    {
        using (var releaser = LockExtensions.EnterReadLock(_readerWriterLock,
                   EnterReadLockTimeout))
        {
            return _jobsByStatus.TryGetValue(status, out var foundJobs)
                ? foundJobs
                : Array.Empty<DataJobDTO>();
        }
    }

    public DataJobDTO GetDataJob(Guid id)
    {
        using (var releaser = LockExtensions.EnterReadLock(_readerWriterLock,
                   EnterReadLockTimeout))
        {
            return _jobsDictionary.TryGetValue(id, out var foundJob)
                ? foundJob
                : throw new JobNotFoundException();
        }
    }

    public DataJobDTO Create(DataJobDTO dataJob)
    {
        using (var releaser = LockExtensions.EnterWriteLock(_readerWriterLock, EnterWriteLockTimeout))
        {
            if (_jobsDictionary.ContainsKey(dataJob.Id))
                throw new InvalidOperationException("This job already exists");
            if (dataJob.Id == default)
                dataJob.Id = Guid.NewGuid();
            
            _jobsDictionary.Add(dataJob.Id, dataJob);

            if (!_jobsByStatus.ContainsKey(dataJob.Status))
            {
                _jobsByStatus.Add(dataJob.Status, new List<DataJobDTO>());
            }
            _jobsByStatus[dataJob.Status].Add(dataJob);

            return dataJob;
        }
    }

    public DataJobDTO Update(DataJobDTO dataJob)
    {
        using (var releaser = LockExtensions.EnterWriteLock(_readerWriterLock, EnterWriteLockTimeout))
        {
            if (!_jobsDictionary.ContainsKey(dataJob.Id))
                throw new InvalidOperationException("This job does not exist");

            var foundJob = _jobsDictionary[dataJob.Id];

            if (foundJob.Status != dataJob.Status)
            {
                if (!_jobsByStatus.ContainsKey(foundJob.Status))
                {
                    throw new JobStatusNotFoundException();
                }

                var jobsWithThisStatus = _jobsByStatus[foundJob.Status];
                jobsWithThisStatus.Remove(foundJob);
            }
            
            foundJob.Links = dataJob.Links;
            foundJob.Name = dataJob.Name;
            foundJob.Results = dataJob.Results;
            foundJob.Status = dataJob.Status;
            foundJob.FilePathToProcess = dataJob.FilePathToProcess;

            return foundJob;
        }
    }

    public void Delete(Guid dataJobID)
    {
        using (var releaser = LockExtensions.EnterWriteLock(_readerWriterLock, EnterWriteLockTimeout))
        {
            if (!_jobsDictionary.ContainsKey(dataJobID))
                throw new InvalidOperationException("This job does not exist");

            var foundJob = _jobsDictionary[dataJobID];
            if (!_jobsByStatus.ContainsKey(foundJob.Status))
                throw new JobStatusNotFoundException();

            var foundJobs = _jobsByStatus[foundJob.Status];
            foundJobs.Remove(foundJob);
            _jobsDictionary.Remove(dataJobID);
        }
    }

    /// <summary>
    /// Starts background process for the specified jobId
    /// </summary>
    /// <param name="dataJobId"></param>
    /// <returns>False if the job is already running, True - otherwise</returns>
    public bool StartBackgroundProcess(Guid dataJobId)
    {
        using (var releaser = LockExtensions.EnterWriteLock(_readerWriterLock, EnterWriteLockTimeout))
        {
            if (!_jobsDictionary.ContainsKey(dataJobId))
                throw new JobNotFoundException();

            var foundJob = _jobsDictionary[dataJobId];
            if (foundJob.Status > DataJobStatus.New)
                return false;

            Task.Run(async () =>
            {
                foundJob.Status = DataJobStatus.Processing;
                await _fileProcessor.ProcessFile(foundJob.FilePathToProcess, 
                    (results) => MarkDataJobAsProcessed(foundJob.Id, results), CancellationToken.None);
            });
            var newJobs = _jobsByStatus[DataJobStatus.New];
            newJobs.Remove(foundJob);
            if (!_jobsByStatus.ContainsKey(DataJobStatus.Processing))
            {
                _jobsByStatus.Add(DataJobStatus.Processing, new List<DataJobDTO>());
            }

            var beingProcessedJobs = _jobsByStatus[DataJobStatus.Processing];
            beingProcessedJobs.Add(foundJob);

            return true;
        }
    }

    private void MarkDataJobAsProcessed(Guid id, IEnumerable<string> results)
    {
        var foundJob = GetDataJob(id);
        using (var releaser = LockExtensions.EnterWriteLock(_readerWriterLock, EnterWriteLockTimeout))
        {
            var jobsWithPrevStatus = _jobsByStatus.TryGetValue(foundJob.Status, out var foundJobs)
                ? foundJobs
                : throw new JobStatusNotFoundException();
            jobsWithPrevStatus.Remove(foundJob);
            if (!_jobsByStatus.ContainsKey(DataJobStatus.Completed))
            {
                _jobsByStatus.Add(DataJobStatus.Completed, new List<DataJobDTO>());
            }
            
            foundJob.Status = DataJobStatus.Completed;
            _jobsByStatus[DataJobStatus.Completed].Add(foundJob);
            foundJob.Results = results;
        }
    }

    public DataJobStatus GetBackgroundProcessStatus(Guid dataJobId)
    {
        var foundJob = GetDataJob(dataJobId);
        return foundJob.Status;
    }

    public List<string> GetBackgroundProcessResults(Guid dataJobId)
    {
        var foundJob = GetDataJob(dataJobId);
        return foundJob.Results.ToList();
    }
}