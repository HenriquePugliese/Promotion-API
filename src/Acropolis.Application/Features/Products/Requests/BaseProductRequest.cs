using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acropolis.Application.Features.Products.Requests
{
    public class BaseProductRequest
    {
        public Guid Id { get; set; }
        public Guid SellerId { get; set; }
        public string? Name { get; set; } = null!;
        public string? MaterialCode { get; set; } = null!;
        public string? UnitMeasure { get; set; } = null!;
        public decimal Weight { get; set; }
        public short Status { get; set; }
        public string? UnitWeight { get; set; } = null!;
    }
}
