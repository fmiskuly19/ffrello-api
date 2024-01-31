using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using test.database;

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
    }
}
