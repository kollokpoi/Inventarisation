using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace Inventarisation.Services
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exp)
            {
                await HandleExceptionAsync(context, exp.GetBaseException());
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exp)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected
            var result = JsonConvert.SerializeObject(new { Code = code, Message = exp.Message, StackTrace = exp.StackTrace });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            using (StreamWriter writer = new StreamWriter("log.txt",true))
            {
                string Data = $"{DateTime.Now} путь:{context.Request.Path} ошибка: {exp.Message}";
                writer.WriteLine(Data);
            }

            return context.Response.WriteAsync(result);
        }
    }
}
