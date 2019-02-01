using Microsoft.AspNetCore.Http;

namespace ticketing_api.Infrastructure
{
    public class UserResolverService
    {
        private readonly IHttpContextAccessor _context;

        public UserResolverService(IHttpContextAccessor context)
        {
            _context = context;
        }

        public string GetUser()
        {
            var user = _context?.HttpContext?.User?.Identity?.Name;

            if (user == null)
            {
                var claims = _context?.HttpContext?.User?.Claims;
                if (claims != null)
                {
                    foreach (var claim in claims)
                    {
                        if (claim.Value == null) continue;
                        user = claim.Value;
                        break;
                    }
                }
            }

            return user ?? "system";
        }
    }
}
