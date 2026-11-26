namespace RecipeNest.Interfaces
{
    public interface IImageStorageService
    {
        Task<string> UploadImageAsync(IFormFile file);

        Task DeleteImageAsync(string imagePath);
    }
}
