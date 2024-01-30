using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test.Models;
using Google.Apis.Auth;
using System.Security.Claims;
using System.Text;
using test.database;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Bson;
using FFrelloApi.Models;

namespace FFrelloApi.Controllers
{
    [Route("api/auth/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public AuthController(IConfiguration configuration) { _configuration = configuration; }
        public class GoogleSignInRequestDto
        {
            public string AccessToken { get; set; } = String.Empty;
        }

        [HttpPost("google-signin")]
        public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInRequestDto request)
        {
            try
            {
                // Assuming request.AccessToken contains the Google access token from the frontend
                var isValidToken = await ValidateGoogleAccessTokenAsync(request.AccessToken);

                if (isValidToken)
                {
                    // Get user information from Google token (for example, email)
                    var googleUser = await GetGoogleUserInfo(request.AccessToken);

                    User? existingUser;
                    using (FfrelloDbContext dbContext = new FfrelloDbContext())
                    {
                        // Check if the user exists in the database
                        existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == googleUser.Email);
                    }

                    //if user exists, generate a jwt and new refresh token, save refresh token to database, return both jwt and refresh token
                    if (existingUser != null)
                    {
                        // User exists, consider authorized

                        // Optionally, generate and return a JWT
                        var jwt = GenerateJwt(existingUser);
                        // generate a refresh token and save to the database
                        var refreshToken = await RotateRefreshToken(existingUser);

                        ////set httponly cookie to use in subsequent api calls
                        //Response.Cookies.Append("accessToken", jwt, new CookieOptions { HttpOnly = true, Secure=true, SameSite = SameSiteMode.None, });
                        //Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.None, });

                        //Response.Headers.Append("Access-Control-Allow-Credentials", "true");

                        return Ok(new { Message = String.Format("Google authentication successful for user {0}", existingUser.Email), AccessToken = jwt, RefreshToken = refreshToken, GoogleUser = googleUser});
                    }

                    //if user didnt exist, create new user, generate jwt and refresh token, return both of those
                    else
                    {
                        var newUser = new User { Email = googleUser.Email }; // Customize based on your User model
                        using (FfrelloDbContext dbContext = new FfrelloDbContext())
                        {
                            dbContext.Users.Add(newUser);
                            await dbContext.SaveChangesAsync();
                        }

                        // Optionally, generate and return a JWT
                        var jwt = GenerateJwt(newUser);
                        // generate a refresh token and save to the database
                        var refreshToken = await RotateRefreshToken(newUser);

                        ////set httponly cookie to use in subsequent api calls
                        //Response.Cookies.Append("accessToken", jwt, new CookieOptions { HttpOnly = true,SameSite=SameSiteMode.None, });
                        //Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions { HttpOnly = true,SameSite = SameSiteMode.None, });

                        return Ok(new { Message = String.Format("Google authentication successful. Created new FFrello user {0}", newUser.Email), AccessToken = jwt, RefreshToken = refreshToken, GoogleUser = googleUser });
                    }
                }
                else
                {
                    return BadRequest(new { Message = "Invalid Google access token" });
                }
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public async Task<string> RotateRefreshToken(User user)
        {
            var refreshToken = Guid.NewGuid().ToString();
            using (FfrelloDbContext dbContext = new FfrelloDbContext())
            {
                // Check if the user exists in the database
                var existingUser = await dbContext.Users.FirstAsync(u => u.Email == user.Email);
                var existingRefreshToken = await dbContext.RefreshTokens.FirstOrDefaultAsync(t => t.User.Email == existingUser.Email);

                //if we found a refresh token for this user that exists, delete it 
                if(existingRefreshToken != null)
                {
                    //I think this is bad? cause we are waiting twice to save, I think this needs to be attached and then one save at the bottom
                    dbContext.RefreshTokens.Remove(existingRefreshToken);
                    await dbContext.SaveChangesAsync();
                }

                //create a new refresh token
                existingUser.RefreshToken = new Models.FFrelloRefreshToken()
                {
                    Token = refreshToken,
                    ExpiryDate = DateTime.UtcNow.AddHours(8),
                };
                dbContext.Attach(existingUser);
                await dbContext.SaveChangesAsync();
            }
            return refreshToken;
        }

        //[HttpPost("refresh-token")]
        //public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        //{
        //    // Validate the refresh token
        //    var isValid = ValidateRefreshToken(request.RefreshToken);

        //    if (!isValid)
        //    {
        //        // Invalid refresh token
        //        return BadRequest(new { message = "Invalid refresh token" });
        //    }

        //    // If the refresh token is valid, extract the user ID and generate a new access token
        //    var userId = ExtractUserIdFromRefreshToken(request.RefreshToken);
        //    var newAccessToken = GenerateAccessToken(userId);

        //    // Return the new access token to the frontend
        //    return Ok(new { accessToken = newAccessToken });
        //}

        //private bool ValidateRefreshToken(string refreshToken)
        //{
        //    // Check if the refresh token exists in the database and is still valid
        //    var existingToken = _dbContext.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshToken);

        //    if (existingToken == null || existingToken.ExpiryDate < DateTime.UtcNow)
        //    {
        //        // Refresh token not found or expired
        //        return false;
        //    }

        //    return true; // Valid refresh token
        //}


        private string GenerateJwt(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_SECRET"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "your_issuer",
                audience: "your_audience",
                claims: claims,
                expires: DateTime.Now.AddHours(4),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet]
        private async Task<bool> ValidateGoogleAccessTokenAsync(string accessToken)
        {
            try
            {
                var validationSettings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { _configuration["GOOGLE_CLIENTID"] } // Replace with your actual Google client ID
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(accessToken, validationSettings);

                // Additional validation logic if needed

                return true;
            }
            catch (InvalidJwtException)
            {
                // The token is invalid
                return false;
            }
        }

        [HttpGet]
        private async Task<GoogleUserInfo> GetGoogleUserInfo(string accessToken)
        {
            try
            {
                var validationSettings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { _configuration["GOOGLE_CLIENTID"] } // Replace with your actual Google client ID
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(accessToken, validationSettings);

                return new GoogleUserInfo
                {
                    Name = payload.Name,
                    PictureUrl = payload.Picture,
                    Email = payload.Email,
                };
            }
            catch (InvalidJwtException)
            {
                // Handle the exception or return null based on your needs
                return null;
            }
        }
    }
}
