using ApiContracts;
using GrpcClient;
using Microsoft.AspNetCore.Mvc;
using UserDTO = ApiContracts.UserDTO;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(UserService.UserServiceClient userService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> CreateUser([FromBody] CreateUserDTO createUser)
    {
        var response = await userService.CreateUserAsync(new CreateUserRequest
        {
            Username = createUser.Username,
            Password = createUser.Password
        });

        return Created($"/users/{response.User.Id}", new UserDTO
        {
            Username = response.User.Username,
            Id = response.User.Id
        });
    }

    [HttpGet("{userId:int}")]
    public async Task<ActionResult<UserDTO>> GetUser(int userId)
    {
        var response = await userService.GetUserByIdAsync(new GetUserByIdRequest { Id = userId });

        return Ok(new UserDTO
        {
            Id = response.User.Id,
            Username = response.User.Username
        });
    }

    [HttpPost("{userId:int}")]
    public async Task<ActionResult<UserDTO>> UpdateUser(int userId, [FromBody] UpdateUserDTO userChanges)
    {
        string userIdClaim = User.FindFirst("Id")!.Value;
        int currentUserId = int.Parse(userIdClaim);
        
        if (currentUserId != userId) return Unauthorized();
        
        // Brugeren vil ændre brugernavn
        if (userChanges.Username is not null)
        {
            await userService.UpdateUsernameAsync(new UpdateUsernameRequest
                { Id = userId, NewUsername = userChanges.Username });
            return await GetUser(userId);
        }

        // Brugeren vil ændre password
        if (userChanges.Password is not null)
        {
            await userService.UpdatePasswordAsync(new UpdatePasswordRequest
                { Id = userId, NewPassword = userChanges.Password });
            return await GetUser(userId);
        }

        return BadRequest("Ingen username eller password");
    }

    [HttpDelete("{userId:int}")]
    public async Task<ActionResult> DeleteUser(int userId)
    {
        string userIdClaim = User.FindFirst("Id")!.Value;
        int currentUserId = int.Parse(userIdClaim);
        
        if (currentUserId != userId) return Unauthorized();
        
        await userService.DeleteUserAsync(new DeleteUserRequest { Id = userId });
        return Ok("Brugeren blev slettet.");
    }
}