using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Shipping.UPS
{
    public class UPSAuthResponse
    {
        public string token_type { get; set; }
        public string issued_at { get; set; }
        public string client_id { get; set; }
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string status { get; set; }
    }
}
