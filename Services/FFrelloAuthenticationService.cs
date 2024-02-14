using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FFrelloApi.database;
using FFrelloApi.Models;

namespace FFrelloApi.Services
{
    public class FFrelloAuthenticationService
    {
        /// <summary>
        /// Checks that the jwt is still valid, checks that the Name ClaimType exists on the token, and that the Claim has a value.
        /// </summary>
        /// <param name="jwtToken">jwt passed from http request</param>
        /// <param name="error">error message if jwt was not valid</param>
        /// <param name="userEmail">userEmail from jwtToken</param>
        /// <returns></returns>
        public bool IsJwtValid(string jwtToken, out string error, out string userEmail)
        {
            error = String.Empty;
            userEmail = String.Empty;
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jsonToken = handler.ReadToken(jwtToken) as JwtSecurityToken;
                if (jsonToken == null || jsonToken.ValidTo < DateTime.UtcNow)
                {
                    error = "Invalid Token. Token is Expired";
                    return false;
                }

                var userEmailClaim = jsonToken?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
                if (userEmailClaim == null || String.IsNullOrEmpty(userEmailClaim.Value))
                {
                    error = "Invalid Token. User email claim not found.";
                    return false;
                }

                //not sure if I neeed to do this
                ////ensure user exists in db as a basic check
                //using (FfrelloDbContext dbContext = new FfrelloDbContext())
                //{
                //    var user = dbContext.Users.FirstOrDefault(x => x.Email == userEmailClaim.Value);
                //    if (user == null)
                //    {
                //        error = "Invalid Token. User not found.";
                //        return false;
                //    }
                //}

                userEmail = userEmailClaim.Value;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GenerateJwt(User user, string secret)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
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
    }
}
