using RangoAgil.API.EndpointFilters;
using RangoAgil.API.EndpointHandlers;

namespace RangoAgil.API.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void MapRangoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var rangos = endpoints.MapGroup("/rangos");
        var rangosWithId = rangos.MapGroup("/{id:int}");
        var rangosWithIdAndFilters = endpoints.MapGroup("/rangos/{id:int}")
            .AddEndpointFilter(new RangoIsLockedFilter(5));

        rangos.MapGet("/", RangoHandlers.GetRangosAsync);
        // Without explicit docs
        rangosWithId.MapGet("", RangoHandlers.GetRangoById).WithName("GetRangoById");

        rangos.MapPost("/", RangoHandlers.CreateRango)
            .AddEndpointFilter<ValidadeAnnotationFilter>();

        rangosWithIdAndFilters.MapPut("", RangoHandlers.UpdateRango);

        rangosWithIdAndFilters.MapDelete("", RangoHandlers.DeleteRango)
            .AddEndpointFilter<LogNotFoundEndpointFilter>();
    }

    public static void MapIngredienteEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var ingredientes = endpoints.MapGroup("/rangos/{id:int}/ingredientes");
        ingredientes.MapGet("/", IngredienteHandlers.GetIngredientesAsync);

        ingredientes.MapPost("/", () =>
        {
            throw new NotImplementedException();
        });
    }
}
