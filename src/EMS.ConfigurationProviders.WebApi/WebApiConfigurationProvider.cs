using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace EMS.ConfigurationProviders.WebApi
{
    public class WebApiConfigurationProvider : ConfigurationProvider
    {
        private readonly WebApiProviderOptions _options;

        public WebApiConfigurationProvider(WebApiProviderOptions options)
        {
            _options = options;
        }

        public override void Load()
        {
            LoadAsync().GetAwaiter().GetResult();
        }

        public async Task LoadAsync(HttpClient httpClient = null)
        {
            if(httpClient == null) httpClient = new HttpClient
            {
                BaseAddress = new Uri(_options.BaseUrl)
            };

            var requestUrl = _options.RequestUrl;
            if (_options.ApiKey != null) requestUrl += $"/{_options.ApiKey}";

            var response = await httpClient.GetStreamAsync(requestUrl);

//            Data = JsonConfigurationFileParser.Parse(response);
            Data = new JsonSerializer().Deserialize<Dictionary<string, string>>(
                new JsonTextReader(new StreamReader(response)));
        }
    }
    
    // Fork from Microsoft.Extensions.Configuration.Json
}