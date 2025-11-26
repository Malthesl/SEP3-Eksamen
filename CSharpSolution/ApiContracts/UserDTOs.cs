namespace ApiContracts;

public class UserDTO
{
    public required int Id { get; set; }
    public required string Username { get; set; }
}

public class CreateUserDTO
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class UpdateUserDTO
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public class UserCredentialsDTO
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class UserTokenDTO
{
    public required string Token { get; set; }
}