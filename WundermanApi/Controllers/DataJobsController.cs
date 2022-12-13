using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace WundermanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataJobsController : ControllerBase
    {
        private readonly IDataProcessorService _dataProcessorService;

        public DataJobsController(IDataProcessorService dataProcessorService)
        {
            _dataProcessorService = dataProcessorService;
        }
        
        //public IActionResult 
    }
}
