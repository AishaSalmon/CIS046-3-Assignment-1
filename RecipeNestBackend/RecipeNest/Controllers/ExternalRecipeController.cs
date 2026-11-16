
using Microsoft.AspNetCore.Mvc;
using RecipeNest.Services.ExternalRecipes;

namespace RecipeNest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalRecipeController : ControllerBase
    {
        private readonly TheMealDbService _mealDbService;

        public ExternalRecipeController(TheMealDbService mealDbService)
        {
            _mealDbService = mealDbService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchRecipes([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Please provide a recipe search term.");
            }

            try
            {
                var recipes = await _mealDbService.SearchRecipesAsync(query);

                return Content(recipes ?? "{}", "application/json");
            }
            catch (HttpRequestException)
            {
                return StatusCode(502, "The external recipe service is currently unavailable.");
            }
        }
    }
}
