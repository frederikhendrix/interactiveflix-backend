namespace ApiGateway.Middleware
{
    public class RoleValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-User-Role", out var roles))
            {
                var role = roles.ToString();
                var path = context.Request.Path.Value;

                // Define role-based access control
                if (IsAdminEndpoint(path))
                {
                    if (role == "Admin")
                    {
                        // Allow access to admin endpoints
                        await _next(context);
                        return;
                    }
                    else
                    {
                        // Deny access
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsync("Access denied. Admin role required.");
                        return;
                    }
                }

                // Allow other roles
                await _next(context);
                return;
            }

            // Deny access if role header is missing
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("User role is required.");
        }

        private bool IsAdminEndpoint(string path)
        {
            // List admin endpoints here
            var adminEndpoints = new[]
            {
            "/post/videometa",
            "/blob/upload",
            };

            foreach (var endpoint in adminEndpoints)
            {
                if (path.StartsWith(endpoint, System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
