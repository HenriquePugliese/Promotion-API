namespace Acropolis.Application.Features.Parameters.Repositories;

public interface IParameterRepository
{
    void AddRange(List<Parameter> parameters);

    void RemoveRange(List<Parameter> parameters);
}