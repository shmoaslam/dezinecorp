using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Shipping
{
    public class QuoteDependentData
    {
        public string Type { get; set; }
        public int? Quantity { get; set; }
        public double? PiecePerCart { get; set; }
        public double? Length { get; set; }
        public double? Width { get; set; }
        public double? Height { get; set; }
        public double? Weight { get; set; }
        public double? CartoonPerSkidLayer { get; set; }
        public string Cartoon { get; set; }
    }
}
