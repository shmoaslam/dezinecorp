using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Shipping.TForce
{
   

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Data
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public int Pieces { get; set; }
        public int Weight { get; set; }
        public double Charge { get; set; }
        public int OfflineCharge { get; set; }
        public double Surcharge { get; set; }
        public double Tax { get; set; }
        public int Accessorial { get; set; }
        public double Subtotal { get; set; }
        public double Total { get; set; }
    }

    public class ResponseDatum
    {
        public string Message { get; set; }
        public Data Data { get; set; }
        public bool status { get; set; }

        public string Error { get; set; }
    }

    public class TForceQuoteResponse
    {
        public List<ResponseDatum> responseData { get; set; }
    }


}
