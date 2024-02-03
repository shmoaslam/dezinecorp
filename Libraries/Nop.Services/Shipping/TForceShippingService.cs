using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Shipping.TForce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Shipping
{
    public class TForceShippingService : ICustomShippingService
    {
        private readonly ShippingConfig _shippingConfig;
        private readonly string _basePath;

        public TForceShippingService(ShippingConfig shippingConfig, string basePath)
        {
            _shippingConfig = shippingConfig;
            _basePath = basePath;
        }

        public void GetShippingQuote()
        {

            try
            {

                HttpContent httpContent = ShippingHelperService.GetJsonContent<TForceQuoteRequest>(_shippingConfig.QuoteJsonFile, _basePath);
                if (httpContent == null) return;

                HttpClient httpClient = new HttpClient();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var httpResponse = httpClient.PostAsync(_shippingConfig.QuoteUrl, httpContent).Result;
                if (!httpResponse.IsSuccessStatusCode)
                    return;

                Console.WriteLine(httpResponse.Content.ReadAsStringAsync().Result);

            }
            catch (Exception)
            {

                throw;
            }



        }
    }
}
