
namespace RangoAgil.API.EndpointFilters;

public class RangoIsLockedFilter(int lockedRangoId) : IEndpointFilter
{
    private readonly int _lockedRangoId = lockedRangoId;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var method = context.HttpContext.Request.Method;

        var rangoId = method switch
        {
            "PUT" => context.GetArgument<int>(2),
            "DELETE" => context.GetArgument<int>(1),
            _ => throw new NotSupportedException($"Método HTTP '{method}' não suportado por este filtro.")
        };

        if (rangoId == _lockedRangoId)
        {
            return TypedResults.Problem(new()
            {
                Status = 400,
                Title = "Rango imutável.",
                Detail = "Você não pode editar nem deletar este registro."
            });
        }

        var result = await next.Invoke(context);
        return result;
    }
}
