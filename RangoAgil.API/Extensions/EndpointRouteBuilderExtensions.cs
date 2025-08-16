using RangoAgil.API.EndpointHandlers;

namespace RangoAgil.API.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void MapRangoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var rangos = endpoints.MapGroup("/rangos");
        var rangosWithId = rangos.MapGroup("/{id:int}");

        rangos.MapGet("/", RangoHandlers.GetRangosAsync);
        // Without explicit docs
        rangosWithId.MapGet("", RangoHandlers.GetRangoById).WithName("GetRangoById");

        rangos.MapPost("/", RangoHandlers.CreateRango);

        rangosWithId.MapPut("", RangoHandlers.UpdateRango);

        rangosWithId.MapDelete("", RangoHandlers.DeleteRango);
    }

    public static void MapIngredienteEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var ingredientes = endpoints.MapGroup("/rangos/{id:int}/ingredientes");
        ingredientes.MapGet("/", IngredienteHandlers.GetIngredientesAsync);
    }
}
