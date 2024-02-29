using Newtonsoft.Json;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Shipping.UPS;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Nop.Core.Domain.Common;

namespace Nop.Services.Shipping
{
    public class UPSShippingService : ICustomShippingService
    {
        private readonly ShippingConfig _shippingConfig;


        public UPSShippingService(ShippingConfig shippingConfig)
        {
            _shippingConfig = shippingConfig;
        }
        public async Task<string> GetShippingQuote(QuoteDezinecorpInput quoteDezinecorpInput)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var authHttpContect = ShippingHelperService.GetFromUrlEncodedContent(_shippingConfig.AuthJsonFile, _shippingConfig.BasePath);

                var byteArray = Encoding.ASCII.GetBytes($"{_shippingConfig.Username}:{_shippingConfig.Password}");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                var authResponseContent = await httpClient.PostAsync(_shippingConfig.AuthUrl, authHttpContect);
                if (authResponseContent == null) return string.Empty;

                var authResponseStr = authResponseContent.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrWhiteSpace(authResponseStr)) return string.Empty;

                var authResponse = JsonConvert.DeserializeObject<UPSAuthResponse>(authResponseStr);
                if (authResponse == null) return string.Empty;

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.access_token);
                HttpContent httpContent = ShippingHelperService.GetJsonContent<UPSQuoteRequest>(_shippingConfig.QuoteJsonFile, _shippingConfig.BasePath);
                if (httpContent == null) return string.Empty;

                var quoteResponseContent = httpClient.PostAsync(_shippingConfig.QuoteUrl, httpContent).Result;
                if (quoteResponseContent == null) return string.Empty;

                var quoteResponseStr = quoteResponseContent.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrWhiteSpace(authResponseStr)) return string.Empty;

                Console.WriteLine(quoteResponseStr);



            }
            catch (Exception ex)
            {

            }
            return string.Empty;
        }
    }
}
