using System.Text.Json;
using System.Text.RegularExpressions;
using backend.Data;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UserController : ControllerBase
{
    private readonly IRepository<User> _userRepository;
    public UserController(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost]
    public async Task<ActionResult<UserDTO>> Create([FromBody] UserCreateDTO userDTO)
    {
        string pattern = @"^(?!\.)[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        if(!Regex.IsMatch(userDTO.Email, pattern))
        {
            return BadRequest("Email does not match a valid pattern");
        }

        var user = new User
        {
            Username = userDTO.Username,
            Email = userDTO.Email,
            Password = HashPassword(userDTO.Password),
            RegistrationDate = DateTime.Now.ToUniversalTime()
        };

        await _userRepository.CreateAsync(user);

        return CreatedAtAction(nameof(Get), new{userId = user.Id}, userDTO);
    }

    [HttpGet(Name = "GetManyUsers")]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetMany([FromQuery] SearchParameters parameters)
    {
        var users = await _userRepository.GetManyAsync(parameters: parameters);

        var previousPageLink = users.HasPrevious ?
            CreateUsersResourceUri(parameters, ResourceUriType.PreviousPage) : null;

        var nextPageLink = users.HasNext ?
            CreateUsersResourceUri(parameters, ResourceUriType.NextPage) : null;

        var paginationMetadata = new
        {
            totalCount = users.TotalCount,
            pageSize = users.PageSize,
            currentPage = users.CurrentPage,
            totalPages = users.TotalPages,
            previousPageLink,
            nextPageLink
        };

        Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));

        //200
        return Ok(users.Select(u => new UserDTO(
            u.Id,
            u.Username,
            u.Email
        )));
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<UserDTO>> Get(int userId)
    {
        var user = await _userRepository.GetAsync(userId: userId);

        if(user == null)
        {
            return NotFound();
        }

        return new UserDTO(user.Id, user.Username, user.Email);
    }

    [HttpPut("{userId}")]
    public async Task<ActionResult<UserDTO>> Update(int userId, [FromBody] UserUpdateDTO userDTO)
    {
        var user = await _userRepository.GetAsync(userId: userId);

        if(user == null)
        {
            return NotFound();
        }

        user.Password = HashPassword(userDTO.Password);

        await _userRepository.UpdateAsync(user);

        return Ok(new UserDTO(user.Id, user.Username, user.Email));
    }

    [HttpDelete("{userId}")]
    public async Task<ActionResult> Remove(int userId)
    {
        var user = await _userRepository.GetAsync(userId: userId);

        if(user == null)
        {
            return NotFound();
        }

        await _userRepository.RemoveAsync(user);

        return NoContent();
    }

    public static string HashPassword(string plainPassword)
    {
        string salt = BCrypt.Net.BCrypt.GenerateSalt(12); 

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword, salt);

        return hashedPassword;
    }

    public static bool VerifyPassword(string plainPassword, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
    }

    private string? CreateUsersResourceUri(
        SearchParameters userSearchParametersDto,
        ResourceUriType type)
    {
        var result = type switch
        {
            ResourceUriType.PreviousPage => Url.Link("GetManyUsers",
                new
                {
                    pageNumber = userSearchParametersDto.PageNumber - 1,
                    pageSize = userSearchParametersDto.PageSize,
                }),
            ResourceUriType.NextPage => Url.Link("GetManyUsers",
                new
                {
                    pageNumber = userSearchParametersDto.PageNumber + 1,
                    pageSize = userSearchParametersDto.PageSize,
                }),
            _ => Url.Link("GetManyUsers",
                new
                {
                    pageNumber = userSearchParametersDto.PageNumber,
                    pageSize = userSearchParametersDto.PageSize,
                })
        };
        return result;
    }
}