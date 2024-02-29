using Nop.Core.Domain.Common;
using Nop.Core.Domain.Shipping;
using System.Threading.Tasks;

namespace Nop.Services.Shipping
{
    public interface ICustomShippingService
    {
        Task<string> GetShippingQuote(QuoteDezinecorpInput quoteDezinecorpInput);
    }
}