
using System.Net.Http;
using System.Threading.Tasks;

namespace RecipeNest.Services.ExternalRecipes
{
    public class TheMealDbService
    {
        private readonly HttpClient _httpClient;

        public TheMealDbService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> SearchRecipesAsync(string query)
        {
            var response = await _httpClient.GetAsync(
                $"search.php?s={Uri.EscapeDataString(query)}"
            );

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
