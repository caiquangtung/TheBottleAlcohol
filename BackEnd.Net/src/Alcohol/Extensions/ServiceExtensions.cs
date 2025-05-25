using System;

namespace Alcohol.Extensions;

public static class ServiceExtensions
{

    public static void ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("ViewUsersPolicy", policy =>
                policy.RequireRole("Admin", "Manager", "CEO"));
        });
    }
}
