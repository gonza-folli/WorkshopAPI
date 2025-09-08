using WorkshopAPI.Interfaces;
using WorkshopAPI.Models;

namespace WorkshopAPI.FunctionClasses
{
    public class ContentValidatorService : IContentValidatorService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ContentValidatorService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<ValidationResult> ValidateContentAsync(string content, string contentType)
        {
            var client = _httpClientFactory.CreateClient("ValidatorFunction");

            var request = new
            {
                Content = content,
                ContentType = contentType
            };

            var response = await client.PostAsJsonAsync("", request);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Validation service returned {response.StatusCode}");
            }

            return await response.Content.ReadFromJsonAsync<ValidationResult>();
        }
    }
}
