namespace Acropolis.Application.Base.Persistence;

public interface IUnitOfWork
{
    Task Commit();
}
