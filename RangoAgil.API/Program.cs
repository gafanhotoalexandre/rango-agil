using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Extensions;
using RangoAgil.API.Profiles;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("RangoDbConStr"))
);

builder.Services.AddAutoMapper(cfg => { }, typeof(RangoAgilProfile));
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsProduction())
{
    app.UseExceptionHandler();

    //Para referência do que pode ser utilizado:
    //app.UseExceptionHandler(configureApplicationBuilder =>
    //{
    //    configureApplicationBuilder.Run(async context =>
    //    {
    //        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    //        context.Response.ContentType = "application/json";
    //        await context.Response.WriteAsync(
    //            "{\"error\":\"An unexpected error occurred. Please try again later.\"}");
    //    });
    //});
}

app.MapRangoEndpoints();
app.MapIngredienteEndpoints();

app.Run();
