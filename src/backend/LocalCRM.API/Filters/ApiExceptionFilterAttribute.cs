using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using LocalCRM.Application.Common.Exceptions;
using System.Diagnostics;

namespace LocalCRM.API.Filters;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        HandleException(context);
        base.OnException(context);
    }

    private void HandleException(ExceptionContext context)
    {
        Type type = context.Exception.GetType();
        if (type == typeof(ConcurrencyException))
        {
            HandleConcurrencyException(context);
            return;
        }

        HandleUnknownException(context);
    }

    private void HandleConcurrencyException(ExceptionContext context)
    {
        var details = new
        {
            code = "concurrency_conflict",
            message = context.Exception.Message,
            traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier
        };

        context.Result = new ConflictObjectResult(details);
        context.ExceptionHandled = true;
    }

    private void HandleUnknownException(ExceptionContext context)
    {
        var details = new
        {
            code = "system_error",
            message = "An unexpected error occurred.",
            traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier
        };

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        context.ExceptionHandled = true;
    }
}
