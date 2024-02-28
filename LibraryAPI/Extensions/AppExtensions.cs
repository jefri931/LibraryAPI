using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;

namespace LibraryAPI.Extensions {
    public static class AppExtensions
    {
        public static void UseHttpLogging(this WebApplication app)
        {
            app.Use(async (context, next) =>
            {
                Log.Information($"Request {context.Request.Method}: {context.Request.Path}");
                await next();
                Log.Information($"Response {context.Response.StatusCode}");
            });
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = exceptionHandlerFeature?.Error;
                    Log.Error(exception, "An unexpected exception occurred.");
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await context.Response.WriteAsync("An unexpected exception occurred.");
                });
            });
        }
    }
}
