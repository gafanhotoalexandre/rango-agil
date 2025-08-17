using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Models;

namespace RangoAgil.API.EndpointHandlers;

public static class IngredienteHandlers
{
    public static async Task<Results<NotFound, NoContent, Ok<IEnumerable<IngredienteDTO>>>> GetIngredientesAsync(
        RangoDbContext dbContext,
        IMapper mapper,
        [FromRoute] int id)
    {
        var rango = await dbContext.Rangos
            .Include(r => r.Ingredientes)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rango is null)
            return TypedResults.NotFound();

        if (rango.Ingredientes is null || rango.Ingredientes.Count == 0)
            return TypedResults.NoContent();

        var ingredientesDto = mapper.Map<IEnumerable<IngredienteDTO>>(rango.Ingredientes);
        return TypedResults.Ok(ingredientesDto);

    }
}
