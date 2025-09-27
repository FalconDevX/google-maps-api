using Microsoft.AspNetCore.Mvc;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserRecommendationController : ControllerBase
    {
        private readonly UserRecommendationService _service;

        public UserRecommendationController(UserRecommendationService service)
        {
            _service = service;
        }

        [HttpGet("fetch")]
        public IActionResult GetPlaces()
        {
            var result = _service.FetchPlaces(); 
            return Ok(result);
        }
    }
}
