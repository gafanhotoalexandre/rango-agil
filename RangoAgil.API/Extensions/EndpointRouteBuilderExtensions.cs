using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RangoAgil.API.EndpointFilters;
using RangoAgil.API.EndpointHandlers;

namespace RangoAgil.API.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void MapRangoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGroup("/identity/").MapIdentityApi<IdentityUser>().WithTags("Identity");

        // Simulating old endpoint
        endpoints.MapGet("/pratos/{pratoId:int}", ([FromRoute] int pratoId) => new
        {
            Message = $"O prato {pratoId} estava supimpa"
        }).WithOpenApi(operation =>
        {
            operation.Deprecated = true;
            return operation;
        }).WithSummary("Este endpoint está deprecated e será descontinuado na versão 2 desta API.");


        var rangos = endpoints.MapGroup("/rangos")
            .RequireAuthorization();
        var rangosWithId = rangos.MapGroup("/{id:int}");
        var rangosWithIdAndFilters = endpoints.MapGroup("/rangos/{id:int}")
            .RequireAuthorization("RequireAdminFromBrazil")
            .AddEndpointFilter(new RangoIsLockedFilter(5))
            .RequireAuthorization();

        rangos.MapGet("/", RangoHandlers.GetRangosAsync)
            .AllowAnonymous();
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
        var ingredientes = endpoints.MapGroup("/rangos/{id:int}/ingredientes")
            .RequireAuthorization();
        ingredientes.MapGet("/", IngredienteHandlers.GetIngredientesAsync);

        ingredientes.MapPost("/", () =>
        {
            throw new NotImplementedException();
        });
    }

    //public static void MapIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    //{
    //    endpoints.MapGroup("/identity/").MapIdentityApi<IdentityUser>().WithTags("Identity");
    //}
}
