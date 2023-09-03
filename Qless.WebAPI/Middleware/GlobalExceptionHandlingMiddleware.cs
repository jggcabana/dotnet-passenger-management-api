using Microsoft.AspNetCore.Mvc;
using Qless.Repositories.Exceptions;
using System.Net;
using System.Text.Json;

namespace Qless.WebAPI.Middleware
{
    public class GlobalExceptionHandlingMiddleware : IMiddleware
    {
		private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
		public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
		{
			_logger = logger;
		}

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
			try
			{
				await next(context);
			}
			catch (QlessException e)
			{
                _logger.LogError(e, e.Message);
				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				
				string json = JsonSerializer.Serialize(e.Message);
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(json);
            }
			catch (Exception e)
			{
				_logger.LogError(e, e.Message);

				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				ProblemDetails problem = new ProblemDetails
				{
					Status = (int)HttpStatusCode.InternalServerError,
					Type = "Server Error",
					Title = "Server Error",
					Detail = "An internal server error occured."
				};

				string json = JsonSerializer.Serialize(problem);
				context.Response.ContentType = "application/json";
				await context.Response.WriteAsync(json);
			}
        }
    }
}
