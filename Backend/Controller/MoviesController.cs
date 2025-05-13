using Microsoft.AspNetCore.Mvc;

namespace Backend.Controller
{
    [Route("api/[controller]")] //case insensitive, will become  api/movies
    [ApiController] //intended to serve HTTP API responses, not Razor views?????? what is Razor view
    public class MoviesController : ControllerBase
    {
        [HttpGet]   // api/movies
        public IEnumerable<string> GetMoviesList()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{id}")]   // api/movies/2
        public string GetMovie(int id)
        {
            return "value";
        }

        [HttpPost]  // api/movies
        public void AddMovie([FromBody] string value)
        {
        }

        [HttpPut("{id}")]   // api/movies/2
        public void UpdateMovie(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]    // api/movies/2
        public void DeleteMovie(int id)
        {
        }
    }
}
