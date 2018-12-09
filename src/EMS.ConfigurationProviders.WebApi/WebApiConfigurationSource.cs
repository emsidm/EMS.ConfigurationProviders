using System;
using Microsoft.Extensions.Configuration;

namespace EMS.ConfigurationProviders.WebApi
{
    public class WebApiConfigurationSource : IConfigurationSource
    {
        private readonly WebApiProviderOptions _options = new WebApiProviderOptions();

        public WebApiConfigurationSource(WebApiProviderOptions options)
        {
            _options = options;
        }
        
        public WebApiConfigurationSource(Action<WebApiProviderOptions> action)
        {
            action(_options);
        }


        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new WebApiConfigurationProvider(_options);
        }
    }
}