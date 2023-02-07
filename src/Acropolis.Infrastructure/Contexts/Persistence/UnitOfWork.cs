using Acropolis.Application.Base.Persistence;
using Acropolis.Infrastructure.Contexts;

namespace Acropolis.Infrastructure.Contexts.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AcropolisContext _context;

    public UnitOfWork(AcropolisContext context)
    {
        _context = context;
    }

    public async Task Commit()
    {
        await _context.SaveChangesAsync();
    }
}
