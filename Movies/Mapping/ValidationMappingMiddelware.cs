using Movies.Contracts.Responses;
using System.ComponentModel.DataAnnotations;

namespace Movies.Api.Mapping
{
    public class ValidationMappingMiddelware
    {
        private readonly RequestDelegate _next;

        public ValidationMappingMiddelware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = 400;
                var validationFailureResponse = new ValidationFailureResponse
                {

                    Errors = ex.Data.Keys.Cast<string>().Select(key => new ValidationResponse
                    {
                        PropertyName = key,
                        Message = ex.Data[key].ToString()
                    }).ToList()

                };

                await context.Response.WriteAsJsonAsync(validationFailureResponse);

            }
        }
    }
}
