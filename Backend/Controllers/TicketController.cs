using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using backend.Data;
using backend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/v1/movies/{movieId}/showings/{showingId}/tickets")]
public class TicketController : ControllerBase
{
    private readonly IRepository<Ticket> _ticketRepository;
    private readonly IRepository<Showing> _showingRepository;
    private readonly IRepository<Movie> _movieRepository;
    private readonly IAuthorizationService _authorizationService;

    public TicketController(IRepository<Ticket> ticketRepository, IRepository<Showing> showingRepository, IRepository<Movie> movieRepository, IAuthorizationService authorizationService)
    {
        _ticketRepository = ticketRepository;
        _showingRepository = showingRepository;
        _movieRepository = movieRepository;
        _authorizationService = authorizationService;
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Viewer)]
    public async Task<ActionResult<TicketDTO>> Create(int movieId, int showingId, [FromBody] TicketCreateDTO ticketDTO)
    {
        var movie = await _movieRepository.GetAsync(movieId);

        if(movie == null)
        {
            return BadRequest($"Movie with the id {movieId} does not exist");
        }

        var showing = await _showingRepository.GetAsync(movieId, showingId);

        if(showing == null)
        {
            return BadRequest($"Showing with the id {showingId} for the movie with the id {movieId} does not exist");
        }

        var tickets = await _ticketRepository.GetAllAsync();

        var maxTicketId = tickets
            .Where(s => s.ShowingNumber == showingId && s.MovieId == movieId)
            .Select(s => s.Id)
            .DefaultIfEmpty(0)
            .Max();

        var ticket = new Ticket
        {
            Id = maxTicketId + 1,
            Showing = showing,
            ShowingNumber = showingId,
            MovieId = movieId,
            TicketType = ticketDTO.TicketType,
            Seat = ticketDTO.Seat,
            UserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
        };

        await _ticketRepository.CreateAsync(ticket);

        return CreatedAtAction(
            nameof(Get), 
            new{ movieId, showingId = ticket.ShowingNumber, ticketId = ticket.Id}, 
            new TicketDTO(ticket.Id, ticket.MovieId, ticket.ShowingNumber, ticket.TicketType, ticket.Seat, ticket.UserId));
    }

    [HttpGet(Name = "GetManyTickets")]
    [Authorize(Roles = UserRoles.Viewer)]
    public async Task<ActionResult<IEnumerable<TicketDTO>>> GetMany([FromQuery] SearchParameters parameters, int movieId = -1, int showingId = -1, bool getAll = false)
    {
        PagedList<Ticket> tickets;

        if(!User.IsInRole(UserRoles.Admin) && !getAll)
        {
            tickets = await (_ticketRepository as TicketRepository)!.GetManyForUserAsync(movieId, showingId, userId: User.FindFirstValue(JwtRegisteredClaimNames.Sub), parameters: parameters);
        }
        else
        {
            tickets = await _ticketRepository.GetManyAsync(movieId, showingId, parameters: parameters);
        }

        var previousPageLink = tickets.HasPrevious ?
            CreateTicketsResourceUri(parameters, ResourceUriType.PreviousPage) : null;

        var nextPageLink = tickets.HasNext ?
            CreateTicketsResourceUri(parameters, ResourceUriType.NextPage) : null;

        var paginationMetadata = new
        {
            totalCount = tickets.TotalCount,
            pageSize = tickets.PageSize,
            currentPage = tickets.CurrentPage,
            totalPages = tickets.TotalPages,
            previousPageLink,
            nextPageLink
        };

        Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));

        //200
        return Ok(tickets.Select(s => new TicketDTO(
            s.Id,
            s.MovieId,
            s.ShowingNumber,
            s.TicketType,
            s.Seat,
            s.UserId
        )));
    }

    [HttpGet("{ticketId}")]
    [Authorize(Roles = UserRoles.Viewer)]
    public async Task<ActionResult<TicketDTO>> Get(int movieId, int showingId, int ticketId)
    {
        var ticket = await _ticketRepository.GetAsync(movieId, showingId, ticketId);

        if(ticket == null)
        {
            return NotFound();
        }

        var authorrizationResult = await _authorizationService.AuthorizeAsync(User, ticket, PolicyNames.ResourceOwner);
        if(!authorrizationResult.Succeeded)
        {
            return Forbid();
        }

        return new TicketDTO(ticket.Id, ticket.MovieId, ticket.ShowingNumber, ticket.TicketType, ticket.Seat, ticket.UserId);
    }

    [HttpPut("{ticketId}")]
    [Authorize(Roles = UserRoles.Viewer)]
    public async Task<ActionResult<TicketDTO>> Update(int movieId, int showingId, int ticketId, [FromBody] TicketUpdateDTO ticketDTO)
    {
        var ticket = await _ticketRepository.GetAsync(movieId, showingId, ticketId);

        if(ticket == null)
        {
            return NotFound();
        }

        var authorrizationResult = await _authorizationService.AuthorizeAsync(User, ticket, PolicyNames.ResourceOwner);
        if(!authorrizationResult.Succeeded)
        {
            return Forbid();
        }

        ticket.TicketType = ticketDTO.TicketType;

        await _ticketRepository.UpdateAsync(ticket);

        return Ok(new TicketDTO(ticket.Id, ticket.MovieId, ticket.ShowingNumber, ticket.TicketType, ticket.Seat, ticket.UserId));
    }

    [HttpDelete("{ticketId}")]
    [Authorize(Roles = UserRoles.Viewer)]
    public async Task<ActionResult> Remove(int movieId, int showingId, int ticketId)
    {
        var ticket = await _ticketRepository.GetAsync(movieId, showingId, ticketId);

        if(ticket == null)
        {
            return NotFound();
        }

        var authorrizationResult = await _authorizationService.AuthorizeAsync(User, ticket, PolicyNames.ResourceOwner);
        if(!authorrizationResult.Succeeded)
        {
            return Forbid();
        }

        var showing = await _showingRepository.GetAsync(movieId, showingId);

        if(showing!.StartTime < DateTime.Now)
        {
            return BadRequest("Can't refund a ticket for a showing that has already started");
        }

        await _ticketRepository.RemoveAsync(ticket);

        return NoContent();
    }

    private string? CreateTicketsResourceUri(
        SearchParameters ticketSearchParametersDto,
        ResourceUriType type)
    {
        var result = type switch
        {
            ResourceUriType.PreviousPage => Url.Link("GetManyTickets",
                new
                {
                    pageNumber = ticketSearchParametersDto.PageNumber - 1,
                    pageSize = ticketSearchParametersDto.PageSize,
                }),
            ResourceUriType.NextPage => Url.Link("GetManyTickets",
                new
                {
                    pageNumber = ticketSearchParametersDto.PageNumber + 1,
                    pageSize = ticketSearchParametersDto.PageSize,
                }),
            _ => Url.Link("GetManyTickets",
                new
                {
                    pageNumber = ticketSearchParametersDto.PageNumber,
                    pageSize = ticketSearchParametersDto.PageSize,
                })
        };
        return result;
    }
}