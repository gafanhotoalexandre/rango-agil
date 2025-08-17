using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Entities;
using RangoAgil.API.Models;

namespace RangoAgil.API.EndpointHandlers;

public static class RangoHandlers
{
    public static async Task<CreatedAtRoute<RangoDTO>> CreateRango(
        RangoDbContext dbContext,
        IMapper mapper,
        //LinkGenerator generator,
        //HttpContext httpContext,
        [FromBody] CreateRangoDTO request)
    {
        var entity = mapper.Map<Rango>(request);
        dbContext.Rangos.Add(entity);

        await dbContext.SaveChangesAsync();

        var rangoResponse = mapper.Map<RangoDTO>(entity);

        return TypedResults.CreatedAtRoute(rangoResponse, "GetRangoById", new { id = entity.Id });

        // Referência..
        //var rangoUri = generator.GetUriByName(
        //    httpContext,
        //    "GetRangoById",
        //    new { id = entity.Id });

        //return TypedResults.Created(rangoUri, rangoResponse);
    }

    public static async Task<Results<NotFound, NoContent>> UpdateRango(
        RangoDbContext dbContext,
        IMapper mapper,
        [FromRoute] int id,
        [FromBody] UpdateRangoDTO request)
    {
        var entity = await dbContext.Rangos.FirstOrDefaultAsync(rango => rango.Id == id);

        if (entity is null)
            return TypedResults.NotFound();

        mapper.Map(request, entity);

        await dbContext.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>> GetRangosAsync(
        RangoDbContext dbContext,
        IMapper mapper,
        ILogger<RangoDTO> logger,
        [FromQuery(Name = "nome")] string? rangoNome)
    {
        var entities = await dbContext.Rangos
                                .Where(r => rangoNome == null || r.Nome.ToLower().Contains(rangoNome.ToLower()))
                                .ToListAsync();

        if (entities.Count <= 0)
        {
            logger.LogInformation($"De acordo com a busca, nenhum rango foi encontrado: {rangoNome}");
            return TypedResults.NoContent();
        }

        logger.LogInformation("Retornando rangos");
        return TypedResults.Ok(mapper.Map<IEnumerable<RangoDTO>>(entities));
    }

    public static async Task<Results<NotFound, Ok<RangoDTO>>> GetRangoById(
        RangoDbContext dbContext,
        IMapper mapper,
        int id)
    {
        var entity = await dbContext.Rangos.FirstOrDefaultAsync(rango => rango.Id == id);
        if (entity is null)
            return TypedResults.NotFound();

        var rangoResponse = mapper.Map<RangoDTO>(entity);

        return TypedResults.Ok(rangoResponse);

    }

    public static async Task<Results<NotFound, NoContent>> DeleteRango(
        RangoDbContext dbContext,
        [FromRoute] int id
        )
    {
        var entity = await dbContext.Rangos.FirstOrDefaultAsync(rango => rango.Id == id);

        if (entity is null)
            return TypedResults.NotFound();

        dbContext.Rangos.Remove(entity);

        await dbContext.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}
