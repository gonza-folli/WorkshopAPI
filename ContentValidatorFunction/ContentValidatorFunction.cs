using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using WorkshopAPI.EF;
using WorkshopAPI.Models;

namespace ContentValidatorFunction
{
    public class ContentValidatorFunction
    {
        private readonly ILogger<ContentValidatorFunction> _logger;

        public ContentValidatorFunction(ILogger<ContentValidatorFunction> logger)
        {
            _logger = logger;
        }

        [Function("ContentValidator")]
        public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                //var requestBody = await req.ReadAsStringAsync();
                //var validationRequest = JsonSerializer.Deserialize<ValidationRequest>(requestBody);

                var validationRequest = await req.ReadFromJsonAsync<ValidationRequest>();

                if (validationRequest == null)
                {
                    return CreateResponse(req, HttpStatusCode.BadRequest, new
                    {
                        Valid = false,
                        Message = "Invalid request format"
                    });
                }

                var person = JsonSerializer.Deserialize<People>(validationRequest.Content);

                if (person == null)
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest);
                }

                var errors = new List<string>();

                if (string.IsNullOrEmpty(person.Name))
                    errors.Add("Name is required");

                if (string.IsNullOrEmpty(person.LastName))
                    errors.Add("LastName is required");

                if (string.IsNullOrEmpty(person.Email))
                    errors.Add("Valid email is required");

                var result = new ValidationResult
                {
                    Valid = errors.Count == 0,
                    Errors = errors,
                };

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(result);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing validation request");
                return CreateResponse(req, HttpStatusCode.InternalServerError, new
                {
                    Valid = false,
                    Message = "Internal server error"
                });
            }
        }

        private HttpResponseData CreateResponse(HttpRequestData req, HttpStatusCode statusCode, object body)
        {
            var response = req.CreateResponse(statusCode);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            response.WriteString(JsonSerializer.Serialize(body));
            return response;
        }
    }
}
