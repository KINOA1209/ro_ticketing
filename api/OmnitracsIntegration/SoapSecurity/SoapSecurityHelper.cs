using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace OmnitracsIntegration.SoapSecurity
{
    public static class SoapSecurityHelper
    {
        public static ChannelFactory<T> CreateSecuredChannel<T>(string url, string username, string password)
        {
            var binding = new BasicHttpsBinding
            {
                TextEncoding = Encoding.UTF8,
                UseDefaultWebProxy = true,
                BypassProxyOnLocal = false,
                Security =
                {
                    Mode = BasicHttpsSecurityMode.Transport,
                    Transport =
                    {
                        ClientCredentialType = HttpClientCredentialType.None,
                        ProxyCredentialType = HttpProxyCredentialType.None
                    }
                }
            };

            var channel = new ChannelFactory<T>(binding, new EndpointAddress(url));
            channel.Endpoint.EndpointBehaviors.Add(new SoapSecurityHeaderBehavior(username, password));

            channel.Credentials.UserName.UserName = username; // not sure if needed
            channel.Credentials.UserName.Password = password;
            return channel;
        }

    }
}
