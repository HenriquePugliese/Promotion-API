using Acropolis.Application.Features.Customers;
using Acropolis.Application.Features.Customers.Repositories;
using Acropolis.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Acropolis.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DbSet<Customer> _customers;

        public CustomerRepository(AcropolisContext context)
        {
            _customers = context.Customers;
        }
        public async Task Add(Customer customer)
        {
            await _customers.AddAsync(customer);
        }

        public async Task<Customer?> FindBy(Expression<Func<Customer, bool>> predicate) => await _customers.AsNoTracking().Include(x => x.Parameters).FirstOrDefaultAsync(predicate);

        public void Remove(Customer customer)
        {
            _customers.Remove(customer);
        }

        public void Update(Customer customer)
        {
            _customers.Update(customer);
        }

        public async Task<Customer?> GetByCustomerCodeAsync(string customerCode, Guid sellerId)
        {
            return await _customers
                   .Include(parameters => parameters.Parameters)
                   .FirstOrDefaultAsync(customer => customer.CustomerCode == customerCode && customer.SellerId == sellerId.ToString());
        }
    }
}
