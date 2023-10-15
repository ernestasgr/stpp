using System.ComponentModel.DataAnnotations;

namespace backend.Data;

/*public record UserCreateDTO(string Username, string Email, string Password);
public record UserUpdateDTO(string Password);
public record UserDTO(int Id, string Username, string Email);*/
public record RegisterUserDTO([Required] string Username, [EmailAddress][Required] string Email, [Required] string Password);
public record LoginUserDTO(string Username, string Password);
public record UserDTO(string Id, string Username, string Email);
public record SuccessfulLoginDTO(string AccessToken);