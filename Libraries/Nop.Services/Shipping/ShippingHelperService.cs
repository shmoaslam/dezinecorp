using DocumentFormat.OpenXml.Bibliography;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Shipping
{
    public class ShippingHelperService 
    {

        private static string GetFileContent(string filePath) => File.ReadAllText(filePath);

        private static T ParseJsonToType<T>(string filePath, string basePath) => JsonConvert.DeserializeObject<T>(GetFileContent(Path.Combine(basePath, filePath)));

        public static HttpContent GetJsonContent<T>(string filePath, string basePath) => new StringContent(JsonConvert.SerializeObject(ParseJsonToType<T>(filePath, basePath)), Encoding.UTF8, "application/json");


        public static HttpContent GetFromUrlEncodedContent(string filePath, string basePath)
        {
            var jsonData = ParseJsonToType<Dictionary<string, string>>(filePath, basePath);
            if (jsonData == null) return null;
            return new FormUrlEncodedContent(jsonData);
        }



    }
}
