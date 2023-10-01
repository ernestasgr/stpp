namespace backend.Data;

public record UserCreateDTO(string Username, string Email, string Password);
public record UserUpdateDTO(string Password);
public record UserDTO(int Id, string Username, string Email);