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

namespace Nop.Services.Shipping
{
    public class UPSShippingService : ICustomShippingService
    {
        private readonly ShippingConfig _shippingConfig;
        private readonly string _basePath;

        public UPSShippingService(ShippingConfig shippingConfig, string basePath)
        {
            _shippingConfig = shippingConfig;
            _basePath = basePath;
        }
        public void GetShippingQuote()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var authHttpContect = ShippingHelperService.GetFromUrlEncodedContent(_shippingConfig.AuthJsonFile, _basePath);

                var byteArray = Encoding.ASCII.GetBytes($"{_shippingConfig.Username}:{_shippingConfig.Password}");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                var authResponseContent = httpClient.PostAsync(_shippingConfig.AuthUrl, authHttpContect).Result;
                if (authResponseContent == null) return;

                var authResponseStr = authResponseContent.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrWhiteSpace(authResponseStr)) return;

                var authResponse = JsonConvert.DeserializeObject<UPSAuthResponse>(authResponseStr);
                if (authResponse == null) return;

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.access_token);
                HttpContent httpContent = ShippingHelperService.GetJsonContent<UPSQuoteRequest>(_shippingConfig.QuoteJsonFile, _basePath);
                if (httpContent == null) return;

                var quoteResponseContent = httpClient.PostAsync(_shippingConfig.QuoteUrl, httpContent).Result;
                if (quoteResponseContent == null) return;

                var quoteResponseStr = quoteResponseContent.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrWhiteSpace(authResponseStr)) return;

                Console.WriteLine(quoteResponseStr);



            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
