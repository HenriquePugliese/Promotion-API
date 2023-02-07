using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acropolis.Application.Features.Promotions.Requests
{
    public class ProductSellerRequest
    {
        public string Id { get; set; } = null!;
        public Guid SellerId { get; set; }
    }
}
