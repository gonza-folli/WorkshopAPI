using WorkshopAPI.Models;

namespace WorkshopAPI.Interfaces
{
    public interface IContentValidatorService
    {
        Task<ValidationResult> ValidateContentAsync(string content, string contentType);
    }
}
