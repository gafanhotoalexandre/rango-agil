using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Extensions;
using RangoAgil.API.Profiles;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("RangoDbConStr"))
);

builder.Services.AddAutoMapper(cfg => { }, typeof(RangoAgilProfile));

var app = builder.Build();

app.MapRangoEndpoints();
app.MapIngredienteEndpoints();

app.Run();
