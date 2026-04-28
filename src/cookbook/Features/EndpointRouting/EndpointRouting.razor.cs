using cookbook.StartupExtensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace cookbook.Features.EndpointRouting;

public partial class EndpointRouting : IRouteDefinition
{
    public IEndpointRouteBuilder MapRoutes(IEndpointRouteBuilder routes)
    {
        routes.MapGet("/endpointrouting", () => new RazorComponentResult<EndpointRouting>());

        return routes;
    }
}
