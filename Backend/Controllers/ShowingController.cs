using backend.Data;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/v1/movies/{movieId}/showings")]
public class ShowingController : ControllerBase
{
    private readonly IRepository<Showing> _showingRepository;
    private readonly IRepository<Movie> _movieRepository;
    public ShowingController(IRepository<Showing> showingRepository, IRepository<Movie> movieRepository)
    {
        _showingRepository = showingRepository;
        _movieRepository = movieRepository;
    }

    [HttpPost]
    public async Task<ActionResult<ShowingDTO>> Create(int movieId, [FromBody] ShowingCreateDTO showingDTO)
    {
        var movie = await _movieRepository.GetAsync(movieId);

        if(movie == null)
        {
            return BadRequest($"Movie with the id {movieId} does not exist");
        }

        if(showingDTO.StartTime.Kind != DateTimeKind.Utc)
        {
            return BadRequest("Start time must match the UTC format");
        }
        if(showingDTO.EndTime.Kind != DateTimeKind.Utc)
        {
            return BadRequest("End time must match the UTC format");
        }
        if(showingDTO.EndTime < showingDTO.StartTime)
        {
            return BadRequest("The showing cannot end earlier than it starts");
        }

        var showings = await _showingRepository.GetAllAsync();

        var maxShowingId = showings
            .Where(s => s.MovieId == movieId)
            .Select(s => s.Number)
            .DefaultIfEmpty(0)
            .Max();

        var showing = new Showing
        {
            Number = maxShowingId + 1,
            StartTime = showingDTO.StartTime,
            EndTime = showingDTO.EndTime,
            MovieId = movieId,
            Movie = movie
        };

        await _showingRepository.CreateAsync(showing);

        return CreatedAtAction(
            nameof(Get), 
            new{movieId = showing.MovieId, showingId = showing.Number}, 
            new ShowingDTO(showing.Number, showing.StartTime, showing.EndTime, showing.MovieId));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShowingDTO>>> GetAll(int movieId = -1)
    {
        var showings = await _showingRepository.GetAllAsync(movieId);
        return Ok(showings.Select(s => new ShowingDTO(
            s.Number,
            s.StartTime,
            s.EndTime,
            s.MovieId
        )));
    }

    [HttpGet("{showingId}")]
    public async Task<ActionResult<ShowingDTO>> Get(int movieId, int showingId)
    {
        var showing = await _showingRepository.GetAsync(movieId, showingId);

        if(showing == null)
        {
            return NotFound();
        }

        return new ShowingDTO(showingId, showing.StartTime, showing.EndTime, movieId);
    }

    [HttpPut("{showingId}")]
    public async Task<ActionResult<ShowingDTO>> Update(int movieId, int showingId, [FromBody] ShowingCreateDTO showingDTO)
    {
        var showing = await _showingRepository.GetAsync(movieId, showingId);

        if(showing == null)
        {
            return NotFound();
        }

        if(showingDTO.StartTime.Kind != DateTimeKind.Utc)
        {
            return BadRequest("Start time must match the UTC format");
        }
        if(showingDTO.EndTime.Kind != DateTimeKind.Utc)
        {
            return BadRequest("End time must match the UTC format");
        }
        if(showingDTO.EndTime < showingDTO.StartTime)
        {
            return BadRequest("The showing cannot end earlier than it starts");
        }

        showing.StartTime = showingDTO.StartTime;
        showing.EndTime = showingDTO.EndTime;

        await _showingRepository.UpdateAsync(showing);

        return Ok(showingDTO);
    }

    [HttpDelete("{showingId}")]
    public async Task<ActionResult> Remove(int movieId, int showingId)
    {
        var showing = await _showingRepository.GetAsync(movieId, showingId);

        if(showing == null)
        {
            return NotFound();
        }

        await _showingRepository.RemoveAsync(showing);

        return NoContent();
    }
}