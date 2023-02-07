using System.Linq.Expressions;

namespace Acropolis.Application.Features.Customers.Repositories;

public  interface ICustomerRepository
{
    Task Add(Customer customer);

    void Update(Customer customer);

    Task<Customer?> FindBy(Expression<Func<Customer, bool>> predicate);

    void Remove(Customer customer);

    Task<Customer?> GetByCustomerCodeAsync(string customerCode, Guid sellerId);
}
