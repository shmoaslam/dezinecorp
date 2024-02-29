using Newtonsoft.Json;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Shipping.TForce;
using Nop.Services.Catalog;
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
        private readonly IInfotracDbService _infotracDbService;
        private readonly string SERVER_ERROR = "Server Error, Unable to get quote";
        private readonly string TFORCE_SERVER_ERROR = "TForce Server Error, Unable to get quote";

        public TForceShippingService(ShippingConfig shippingConfig, IInfotracDbService infotracDbService)
        {
            _shippingConfig = shippingConfig;
            _infotracDbService = infotracDbService;
        }

        public async Task<string> GetShippingQuote(QuoteDezinecorpInput quoteDezinecorpInput)
        {

            try
            {
                var inputData = ShippingHelperService.ParseJsonToType<TForceQuoteRequest>(_shippingConfig.QuoteJsonFile, _shippingConfig.BasePath);
                if (inputData == null) throw new Exception(SERVER_ERROR);

                var rates = inputData.getRates.First();
                if (rates == null) throw new Exception(SERVER_ERROR);

                rates.State = quoteDezinecorpInput.State;
                rates.Zip = quoteDezinecorpInput.ZipCode;

                var productInfo = (await _infotracDbService.GetDimentionData(quoteDezinecorpInput.ProductNumber, quoteDezinecorpInput.Quantity));
                if (productInfo == null) throw new Exception(SERVER_ERROR);
                if (productInfo.Count() > 1) throw new Exception(SERVER_ERROR);
                var p = productInfo.FirstOrDefault();
                if (p == null) throw new Exception(SERVER_ERROR);
                if (p.Height == null || p.Width == null || p.Length == null || p.Weight == null || p.PiecePerCart == null) throw new Exception(SERVER_ERROR);

                if (p.Quantity.HasValue)
                    rates.Pieces = p.Quantity.Value;
                if (p.Weight.HasValue)
                    rates.Weight = rates.Pieces * p.Weight.Value;

                rates.Accessorials = quoteDezinecorpInput.IsResidentialAddress ? "HD" : "";

                var type = rates.Package.FirstOrDefault()?.type;
                rates.Package.Clear();
                for (int i = 0; i < rates.Pieces; i++)
                    rates.Package.Add(new Package
                    {
                        height = p.Height.HasValue ? p.Height.Value : 0,
                        length = p.Length.HasValue ? p.Length.Value : 0,
                        width = p.Width.HasValue ? p.Width.Value : 0,
                        weight = p.Weight.HasValue ? p.Weight.Value : 0,
                        type = type
                    });



                HttpContent httpContent = ShippingHelperService.GetJsonContent(inputData);
                if (httpContent == null) throw new Exception(TFORCE_SERVER_ERROR);

                HttpClient httpClient = new HttpClient();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var httpResponse = httpClient.PostAsync(_shippingConfig.QuoteUrl, httpContent).Result;
                if (!httpResponse.IsSuccessStatusCode)
                    throw new Exception(TFORCE_SERVER_ERROR);

                var response = httpResponse.Content.ReadAsStringAsync().Result;
                var responseObj = JsonConvert.DeserializeObject<TForceQuoteResponse>(response);
                if (responseObj == null) throw new Exception(TFORCE_SERVER_ERROR);

                var responseData = responseObj.responseData.FirstOrDefault();
                if (responseData == null) throw new Exception(TFORCE_SERVER_ERROR);

                if (!responseData.status)
                    throw new Exception(responseData.Error);

                float factor = 1;
                var freightFactors = (await _infotracDbService.GetInfotracFreightFactors());

                var carrierFactor = freightFactors.FirstOrDefault(x => x.Section == "CarrierFactors" && x.Key == "TForce");
                if (carrierFactor == null) throw new Exception(SERVER_ERROR);

                factor = carrierFactor.Data.HasValue ? carrierFactor.Data.Value : 1;

                var provinceFactors = freightFactors.FirstOrDefault(x => x.Section == "ProvinceFactors" && x.Key == quoteDezinecorpInput.State);
                if (provinceFactors == null)
                    provinceFactors = freightFactors.FirstOrDefault(x => x.Section == "ProvinceFactors" && x.Key == "Default");
                if (provinceFactors == null) throw new Exception(SERVER_ERROR);

                factor = provinceFactors.Data.HasValue ? factor * provinceFactors.Data.Value : 1;

                return Math.Round((responseData.Data.Subtotal * factor), 2).ToString();

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    }
}
