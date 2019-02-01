using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace OmnitracsIntegration.SoapSecurity
{
    public class SoapSecurityHeaderBehavior : IEndpointBehavior
    {
        private readonly string _username;
        private readonly string _password;

        public SoapSecurityHeaderBehavior(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new SoapSecurityHeaderInspector(_username, _password));
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {

        }

        public void Validate(ServiceEndpoint endpoint)
        {

        }
    }
}
