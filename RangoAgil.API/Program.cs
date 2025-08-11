using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Models;
using RangoAgil.API.Profiles;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("RangoDbConStr"))
);

builder.Services.AddAutoMapper(cfg => { }, typeof(RangoAgilProfile));


var app = builder.Build();

app.MapGet("/", () => new { Name = "Alexandre", Lastname = "Martins", Nickname = "Lelê" });

app.MapGet("/rangos", async Task<Results<NoContent, Ok<IEnumerable<RangoDTO>>>> (
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
app.MapGet("/rangos/{rangoId:int}/ingredientes", async (
    RangoDbContext dbContext,
    IMapper mapper,
    int rangoId) =>
{
    return mapper.Map<IEnumerable<IngredienteDTO>>((await dbContext.Rangos
                            .Include(rango => rango.Ingredientes)
                            .FirstOrDefaultAsync(rango => rango.Id == rangoId))?.Ingredientes);
});

// Without explicit docs
app.MapGet("/rangos/{id:int}", async (
    RangoDbContext dbContext,
    IMapper mapper,
    int id) =>
{
    return mapper.Map<RangoDTO>(await dbContext.Rangos.FirstOrDefaultAsync(rango => rango.Id == id));
});

app.Run();
