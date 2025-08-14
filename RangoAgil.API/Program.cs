using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Entities;
using RangoAgil.API.Models;
using RangoAgil.API.Profiles;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("RangoDbConStr"))
);

builder.Services.AddAutoMapper(cfg => { }, typeof(RangoAgilProfile));


var app = builder.Build();

var rangos = app.MapGroup("/rangos");
var rangosWithId = rangos.MapGroup("/{id:int}");

app.MapGet("/", () => new { Name = "Alexandre", Lastname = "Martins", Nickname = "Lelê" });

rangos.MapGet("/", async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>> (
    RangoDbContext dbContext,
    IMapper mapper,
    [FromQuery(Name = "nome")] string? rangoNome) =>
{
    var entities = await dbContext.Rangos
                            .Where(r => rangoNome == null || r.Nome.ToLower().Contains(rangoNome.ToLower()))
                            .ToListAsync();

    if (entities.Count <= 0)
        return TypedResults.NoContent();

    return TypedResults.Ok(mapper.Map<IEnumerable<RangoDTO>>(entities));
});

// Endpoint to get Ingredientes by RangoId
rangosWithId.MapGet("/ingredientes", async (
    RangoDbContext dbContext,
    IMapper mapper,
    int id) =>
{
    var rango = await dbContext.Rangos
        .Include(r => r.Ingredientes)
        .FirstOrDefaultAsync(r => r.Id == id);

    if (rango is null)
        return Results.NotFound();

    if (rango.Ingredientes is null || rango.Ingredientes.Count == 0)
        return Results.NoContent();

    var ingredientesDto = mapper.Map<IEnumerable<IngredienteDTO>>(rango.Ingredientes);
    return Results.Ok(ingredientesDto);

});

// Without explicit docs
rangosWithId.MapGet("", async Task<Results<NotFound, Ok<RangoDTO>>> (
    RangoDbContext dbContext,
    IMapper mapper,
    int id) =>
{
    var entity = await dbContext.Rangos.FirstOrDefaultAsync(rango => rango.Id == id);
    if (entity is null)
        return TypedResults.NotFound();

    var rangoResponse = mapper.Map<RangoDTO>(entity);

    return TypedResults.Ok(rangoResponse);

}).WithName("GetRangoById");

rangos.MapPost("/", async Task<CreatedAtRoute<RangoDTO>> (
    RangoDbContext dbContext,
    IMapper mapper,
    //LinkGenerator generator,
    //HttpContext httpContext,
    [FromBody] CreateRangoDTO request) =>
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
});

rangosWithId.MapPut("", async Task<Results<NotFound, NoContent>> (
    RangoDbContext dbContext,
    IMapper mapper,
    [FromRoute] int id,
    [FromBody] UpdateRangoDTO request) =>
{
    var entity = await dbContext.Rangos.FirstOrDefaultAsync(rango => rango.Id == id);

    if (entity is null)
        return TypedResults.NotFound();

    mapper.Map(request, entity);

    await dbContext.SaveChangesAsync();

    return TypedResults.NoContent();
});

rangosWithId.MapDelete("", async Task<Results<NotFound, NoContent>> (
    RangoDbContext dbContext,
    [FromRoute] int id
    ) =>
{
    var entity = await dbContext.Rangos.FirstOrDefaultAsync(rango => rango.Id == id);

    if (entity is null)
        return TypedResults.NotFound();

    dbContext.Rangos.Remove(entity);

    await dbContext.SaveChangesAsync();

    return TypedResults.NoContent();
});

app.Run();
