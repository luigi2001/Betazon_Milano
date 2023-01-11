using Microsoft.AspNetCore.Authorization;

namespace AutoAPI.autenticator
{
    public class BasicAutorizationAttribute : AuthorizeAttribute
    {
        public BasicAutorizationAttribute()
        {
            Policy = "BasicAuthentication";
        }
    }
}
