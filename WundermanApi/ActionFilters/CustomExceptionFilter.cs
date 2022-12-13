using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Model.Exceptions;

namespace WundermanApi.ActionFilters;

public sealed class CustomExceptionFilter : ExceptionFilterAttribute
{
    private readonly ILogger _logger;

    public CustomExceptionFilter(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<CustomExceptionFilter>();
    }
    
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is JobNotFoundException)
        {
            _logger.LogInformation("Resource not found");
            context.Result = new StatusCodeResult(404);
        }
        else if (context.Exception is OperationCanceledException)
        {
            _logger.LogInformation("Request was cancelled");
            context.ExceptionHandled = true;
            context.Result = new StatusCodeResult(499);
        }
        else if (context.Exception is CriticalException)
        {
            _logger.LogError(context.Exception.Message);
            context.Result = new StatusCodeResult(500);
        }
    }
}