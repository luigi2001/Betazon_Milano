using Betazon_Milano.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace AutoAPI.autenticator
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
            ) : base(options, logger, encoder, clock) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Response.Headers.Add("WWW-Authenticate", "Basic");

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Autorizzazione mancante"));
            }

            var authorizationHeader = Request.Headers["Authorization"].ToString();

            var authoHeaderRegEx = new Regex("Basic (.*)");

            if (!authoHeaderRegEx.IsMatch(authorizationHeader))
            {
                return Task.FromResult(AuthenticateResult.Fail("Authorization Code, not properly formatted"));
            }

            var authBase64 = Encoding.UTF8.GetString(Convert.FromBase64String(authoHeaderRegEx.Replace(authorizationHeader, "$1")));
            var authSplit = authBase64.Split(Convert.ToChar(":"), 2);

            var authUser = authSplit[0];
            var authPassword = authSplit.Length > 1 ? authSplit[1] : throw new Exception("Unable to get Password");

            bool VerifyPassword(string password, string hash, string salt)
            {
                SHA512 sHA512 = SHA512.Create();
                sHA512.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
                if (Convert.ToBase64String(sHA512.Hash) == hash)
                {
                    return true;
                }

                return false;
            }

            bool verifica = false;
            var builder = WebApplication.CreateBuilder();
            using (SqlConnection connection = new SqlConnection(builder.Configuration.GetConnectionString("AdventureWorksLT2019").ToString()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Parameters.AddWithValue("@mail", authUser);
                    command.CommandText = $"select * from SalesLT.Customer c where c.EmailAddress = @mail";
                    command.Connection = connection;
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (VerifyPassword(authPassword, reader["PasswordHash"].ToString(), reader["PasswordSalt"].ToString()) && authUser == reader["EmailAddress"].ToString())
                            {
                                verifica = true;
                            }
                        }
                    }
                    reader.Close();
                }
            }

            if (!verifica)
            {
                return Task.FromResult(AuthenticateResult.Fail("User e/o password errati !!!"));
            }

            var authenticatedUser = new AuthenticatedUser("BasicAuthentication", true, authUser);

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(authenticatedUser));

            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
        }
    }
}
