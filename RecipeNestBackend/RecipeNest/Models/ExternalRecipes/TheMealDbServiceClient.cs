using System.Net.Http;
using System.Threading.Tasks;

namespace RecipeNest.Services.ExternalRecipes
{
    public class TheMealDbServiceClient : TheMealDbService
    {
        public TheMealDbServiceClient(HttpClient httpClient) : base(httpClient) {}
    }
}