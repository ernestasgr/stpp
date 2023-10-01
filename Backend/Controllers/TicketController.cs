using backend.Data;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/v1/movies/{movieId}/showings/{showingId}/tickets")]
public class TicketController : ControllerBase
{
    private readonly IRepository<Ticket> _ticketRepository;
    private readonly IRepository<Showing> _showingRepository;
    private readonly IRepository<Movie> _movieRepository;
    public TicketController(IRepository<Ticket> ticketRepository, IRepository<Showing> showingRepository, IRepository<Movie> movieRepository)
    {
        _ticketRepository = ticketRepository;
        _showingRepository = showingRepository;
        _movieRepository = movieRepository;
    }

    [HttpPost]
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
            Price = ticketDTO.Price,
            Showing = showing,
            ShowingNumber = showingId,
            MovieId = movieId,
            TicketType = ticketDTO.TicketType
        };

        await _ticketRepository.CreateAsync(ticket);

        return CreatedAtAction(
            nameof(Get), 
            new{ movieId, showingId = ticket.ShowingNumber, ticketId = ticket.Id}, 
            new TicketDTO(ticket.Id, ticket.Price, ticket.MovieId, ticket.ShowingNumber, ticket.TicketType));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketDTO>>> GetAll(int movieId = -1, int showingId = -1)
    {
        var tickets = await _ticketRepository.GetAllAsync(movieId, showingId);
        return Ok(tickets.Select(s => new TicketDTO(
            s.Id,
            s.Price,
            s.MovieId,
            s.ShowingNumber,
            s.TicketType
        )));
    }

    [HttpGet("{ticketId}")]
    public async Task<ActionResult<TicketDTO>> Get(int movieId, int showingId, int ticketId)
    {
        var ticket = await _ticketRepository.GetAsync(movieId, showingId, ticketId);

        if(ticket == null)
        {
            return NotFound();
        }

        return new TicketDTO(ticket.Id, ticket.Price, ticket.MovieId, ticket.ShowingNumber, ticket.TicketType);
    }

    [HttpPut("{ticketId}")]
    public async Task<ActionResult<TicketDTO>> Update(int movieId, int showingId, int ticketId, [FromBody] TicketUpdateDTO ticketDTO)
    {
        var ticket = await _ticketRepository.GetAsync(movieId, showingId, ticketId);

        if(ticket == null)
        {
            return NotFound();
        }

        ticket.TicketType = ticketDTO.TicketType;

        await _ticketRepository.UpdateAsync(ticket);

        return Ok(ticketDTO);
    }

    [HttpDelete("{ticketId}")]
    public async Task<ActionResult> Remove(int movieId, int showingId, int ticketId)
    {
        var ticket = await _ticketRepository.GetAsync(movieId, showingId, ticketId);

        if(ticket == null)
        {
            return NotFound();
        }

        if(ticket.Showing.StartTime < DateTime.Now)
        {
            return BadRequest("Can't refund a ticket for a showing that has already started");
        }

        await _ticketRepository.RemoveAsync(ticket);

        return NoContent();
    }
}