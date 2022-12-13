namespace Model;

public interface IDataProcessorService
{
    /// <summary>
    /// Returns all available jobs
    /// </summary>
    /// <returns>jobs array</returns>
    IEnumerable<DataJobDTO> GetAllDataJobs();
    
    /// <summary>
    /// Retrieve jobs for a particular status
    /// </summary>
    /// <param name="status"></param>
    /// <returns>IEnumerable of DataJobDTO</returns>
    IEnumerable<DataJobDTO> GetDataJobsByStatus(DataJobStatus status);
    
    /// <summary>
    /// Retrieve data job by its id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    DataJobDTO GetDataJob(Guid id);
    
    /// <summary>
    /// Create a new data job
    /// </summary>
    /// <param name="dataJob"></param>
    /// <returns></returns>
    DataJobDTO Create(DataJobDTO dataJob);
    
    /// <summary>
    /// Update an existing data job
    /// </summary>
    /// <param name="dataJob"></param>
    /// <returns></returns>
    DataJobDTO Update(DataJobDTO dataJob);
    
    /// <summary>
    /// Delete an existing data job
    /// </summary>
    /// <param name="dataJobID"></param>
    void Delete(Guid dataJobID);
    
    /// <summary>
    /// Start an existing data job
    /// </summary>
    /// <param name="dataJobId"></param>
    /// <returns></returns>
    bool StartBackgroundProcess(Guid dataJobId);
    
    /// <summary>
    /// Retrieve the status of a data job
    /// </summary>
    /// <param name="dataJobId"></param>
    /// <returns></returns>
    DataJobStatus GetBackgroundProcessStatus(Guid dataJobId);
    
    /// <summary>
    /// Retrieve results of a data job
    /// </summary>
    /// <param name="dataJobId"></param>
    /// <returns></returns>
    List<string> GetBackgroundProcessResults(Guid dataJobId);
}

