using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using backend.Data;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = UserRoles.Admin)]
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
            Movie = movie,
            Price = showingDTO.Price,
            UserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
        };

        await _showingRepository.CreateAsync(showing);

        return CreatedAtAction(
            nameof(Get), 
            new{movieId = showing.MovieId, showingId = showing.Number}, 
            new ShowingDTO(showing.Number, showing.StartTime, showing.EndTime, showing.MovieId, showing.Price));
    }

    [HttpGet(Name = "GetManyShowings")]
    public async Task<ActionResult<IEnumerable<ShowingDTO>>> GetMany([FromQuery] SearchParameters parameters, int movieId = -1)
    {
        var showings = await _showingRepository.GetManyAsync(movieId, parameters: parameters);

        var previousPageLink = showings.HasPrevious ?
            CreateShowingsResourceUri(parameters, ResourceUriType.PreviousPage) : null;

        var nextPageLink = showings.HasNext ?
            CreateShowingsResourceUri(parameters, ResourceUriType.NextPage) : null;

        var paginationMetadata = new
        {
            totalCount = showings.TotalCount,
            pageSize = showings.PageSize,
            currentPage = showings.CurrentPage,
            totalPages = showings.TotalPages,
            previousPageLink,
            nextPageLink
        };

        Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));

        //200
        return Ok(showings.Select(s => new ShowingDTO(
            s.Number,
            s.StartTime,
            s.EndTime,
            s.MovieId,
            s.Price
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

        return new ShowingDTO(showingId, showing.StartTime, showing.EndTime, movieId, showing.Price);
    }

    [HttpPut("{showingId}")]
    [Authorize(Roles = UserRoles.Admin)]
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
        showing.Price = showingDTO.Price;

        await _showingRepository.UpdateAsync(showing);

        return Ok(new ShowingDTO(showingId, showing.StartTime, showing.EndTime, movieId, showing.Price));
    }

    [HttpDelete("{showingId}")]
    [Authorize(Roles = UserRoles.Admin)]
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

    private string? CreateShowingsResourceUri(
        SearchParameters showingSearchParametersDto,
        ResourceUriType type)
    {
        var result = type switch
        {
            ResourceUriType.PreviousPage => Url.Link("GetManyShowings",
                new
                {
                    pageNumber = showingSearchParametersDto.PageNumber - 1,
                    pageSize = showingSearchParametersDto.PageSize,
                }),
            ResourceUriType.NextPage => Url.Link("GetManyShowings",
                new
                {
                    pageNumber = showingSearchParametersDto.PageNumber + 1,
                    pageSize = showingSearchParametersDto.PageSize,
                }),
            _ => Url.Link("GetManyShowings",
                new
                {
                    pageNumber = showingSearchParametersDto.PageNumber,
                    pageSize = showingSearchParametersDto.PageSize,
                })
        };
        return result;
    }
}