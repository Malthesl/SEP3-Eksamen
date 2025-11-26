using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ApiContracts;
using GrpcClient;
using Microsoft.IdentityModel.Tokens;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(UserService.UserServiceClient userService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<UserTokenDTO>> Login([FromBody] UserCredentialsDTO credentials)
    {
        VerifyUserCredentialsResponse verification = await userService.VerifyUserCredentialsAsync(
            new VerifyUserCredentialsRequest
            {
                Username = credentials.Username,
                Password = credentials.Password
            });

        if (verification.User is null) return Unauthorized();

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, verification.User.Username),
            new Claim("Id", verification.User.Id.ToString(), ClaimValueTypes.Integer)
        };

        var key = new SymmetricSecurityKey("SuperSecretKeyThatIsAtMinimum32CharactersLong"u8.ToArray());
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "QuizPlusPlus",
            audience: "Quizzers",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { token = tokenString });
    }
}