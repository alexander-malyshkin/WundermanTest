namespace Model;

public interface IDataProcessorService
{
    IEnumerable<DataJobDTO> GetAllDataJobs();
    IEnumerable<DataJobDTO> GetDataJobsByStatus(DataJobStatus status);
    DataJobDTO GetDataJob(Guid id);
    DataJobDTO Create(DataJobDTO dataJob);
    DataJobDTO Update(DataJobDTO dataJob);
    void Delete(Guid dataJobID);
    bool StartBackgroundProcess(Guid dataJobId);
    DataJobStatus GetBackgroundProcessStatus(Guid dataJobId);
    List<string> GetBackgroundProcessResults(Guid dataJobId);
}

