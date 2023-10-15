using System.Text.Json;
using System.Text.RegularExpressions;
using backend.Data;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/v1")]
[AllowAnonymous]
public class UserController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    public UserController(UserManager<User> userManager, IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(RegisterUserDTO registerUserDTO)
    {
        var user = await _userManager.FindByNameAsync(registerUserDTO.Username);
        if(user != null)
        {
            return BadRequest("A user with that username already exists");
        }

        var newUser = new User
        {
            UserName = registerUserDTO.Username,
            Email = registerUserDTO.Email
        };

        var createUserResult = await _userManager.CreateAsync(newUser, registerUserDTO.Password);
        if(!createUserResult.Succeeded)
        {
            return BadRequest("Could not create a user");
        }

        await _userManager.AddToRoleAsync(newUser, UserRoles.Viewer);

        return CreatedAtAction(nameof(Register), new UserDTO(newUser.Id, newUser.UserName, newUser.Email));
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginUserDTO loginUserDTO)
    {
        var user = await _userManager.FindByNameAsync(loginUserDTO.Username);
        if(user == null)
        {
            return BadRequest("A user with that username or password does not exist");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginUserDTO.Password);
        if(!isPasswordValid)
        {
            return BadRequest("A user with that username or password does not exist");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenService.CreateAccessToken(user.UserName, user.Id, roles);

        return Ok(new SuccessfulLoginDTO(accessToken));
    }
}