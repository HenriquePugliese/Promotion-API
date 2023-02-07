using Acropolis.Application.Features.Parameters;
using Acropolis.Application.Features.Parameters.Repositories;
using Acropolis.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Acropolis.Infrastructure.Repositories;

public class ParameterRepository : IParameterRepository
{
    private readonly DbSet<Parameter> _parameters;

    public ParameterRepository(AcropolisContext context)
    {
        _parameters = context.Parameters;
    }

    public void AddRange(List<Parameter> parameters)
    {
        _parameters.AddRange(parameters);
    }

    public void RemoveRange(List<Parameter> parameters)
    {
        _parameters.RemoveRange(parameters);
    }
}