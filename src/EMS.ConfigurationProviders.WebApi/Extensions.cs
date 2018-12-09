using System;
using Microsoft.Extensions.Configuration;

namespace EMS.ConfigurationProviders.WebApi
{
    public static class Extensions
    {
        public static IConfigurationBuilder AddWebApi(
            this IConfigurationBuilder builder,
            Action<WebApiProviderOptions> options)
        {
            return builder.Add(new WebApiConfigurationSource(options));
        }
        public static IConfigurationBuilder AddWebApi(
            this IConfigurationBuilder builder,
            WebApiProviderOptions options)
        {
            return builder.Add(new WebApiConfigurationSource(options));
        }
    }
}