using cookbook.StartupExtensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace cookbook.Features.NotFound;

public partial class NotFound : IRouteDefinition
{
    public IEndpointRouteBuilder MapRoutes(IEndpointRouteBuilder routes)
    {
        routes.MapGet("/notfound", () => new RazorComponentResult<NotFound>());

        return routes;
    }

    
}
