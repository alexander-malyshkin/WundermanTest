using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace WundermanApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class DataJobsController : ControllerBase
    {
        private readonly IDataProcessorService _dataProcessorService;

        public DataJobsController(IDataProcessorService dataProcessorService)
        {
            _dataProcessorService = dataProcessorService;
        }

        [HttpGet("dataJobs")]
        public ValueTask<DataJobDTO[]> GetAllDataJobs(CancellationToken ct)
        {
            var allJobs = _dataProcessorService.GetAllDataJobs().ToArray();
            return ValueTask.FromResult(allJobs);
        }

        [HttpGet("dataJobs/{status}")]
        public ValueTask<DataJobDTO[]> GetDataJobsByStatus(DataJobStatus status)
        {
            var jobs = _dataProcessorService.GetDataJobsByStatus(status).ToArray();
            return ValueTask.FromResult(jobs);
        }

        [HttpGet("dataJob/{id}")]
        public ValueTask<DataJobDTO> GetDataJob(Guid id, CancellationToken ct)
        {
            return ValueTask.FromResult(_dataProcessorService.GetDataJob(id));
        }

        [HttpPost("dataJob")]
        public async ValueTask<ActionResult<DataJobDTO>> CreateJob([FromBody] DataJobDTO dataJob,
            CancellationToken ct)
        {
            try
            {
                var createdJob = _dataProcessorService.Create(dataJob);
                return createdJob;
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("dataJob")]
        public async ValueTask<ActionResult<DataJobDTO>> UpdateJob([FromBody] DataJobDTO dataJob,
            CancellationToken ct)
        {
            try
            {
                var updatedJob = _dataProcessorService.Update(dataJob);
                return updatedJob;
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpDelete("dataJob/{jobId}")]
        public async ValueTask<ActionResult> DeleteJob(Guid jobId,
            CancellationToken ct)
        {
            try
            {
                _dataProcessorService.Delete(jobId);
                return Ok();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("dataJob/start/{jobId}")]
        public async ValueTask<ActionResult> StartJob(Guid jobId,
            CancellationToken ct)
        {
            _dataProcessorService.StartBackgroundProcess(jobId);
            return Ok();
        }

        [HttpGet("dataJob/{jobId}/status")]
        public ValueTask<DataJobStatus> GetBackgroundProcessStatus(Guid jobId,
            CancellationToken ct)
        {
            return ValueTask.FromResult(_dataProcessorService.GetBackgroundProcessStatus(jobId));
        }

        [HttpGet("dataJob/{jobId}/results")]
        public ValueTask<List<string>> GetBackgroundProcessResults(Guid jobId,
            CancellationToken ct)
        {
            return ValueTask.FromResult(_dataProcessorService.GetBackgroundProcessResults(jobId));
        }
    }
}
