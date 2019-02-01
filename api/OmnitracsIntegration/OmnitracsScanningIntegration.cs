using OmnitracsIntegration.SoapSecurity;
using System;
using System.Threading.Tasks;
using EIPOutMsgSvcsReference;

namespace OmnitracsIntegration
{
    public class OmnitracsScanningIntegration
    {
        private static OmnitracsScanningIntegration _instance;
        public static OmnitracsScanningIntegration Instance => _instance ?? (_instance = new OmnitracsScanningIntegration());

        public async Task<getMessagesCountResponse> GetMessageCountAsync()
        {
            var factory = SoapSecurityHelper.CreateSecuredChannel<EIPOutMsgSvcs10>(OmnitracsConfiguration.EipWebSvcsEndpoint, OmnitracsConfiguration.Username, OmnitracsConfiguration.Password);
            var serviceProxy = factory.CreateChannel();

            var request = new getMessagesCountRequest();
            request.StartDate = DateTime.Now.AddDays(-7);
            //request.EndDate = new DateTime();
            var routing = new RoutingType();
            var service = new ServiceType {Primary = new IdentityType {Id = "SCAN"}};
            routing.Service = new[] { service };
            request.Routing = routing;
            getMessagesCountResponse response;
            try
            {
                response = await serviceProxy.getMessagesCountAsync(request);
            }
            catch (Exception ex)
            {
                throw;
            }

            factory.Close();

            return response;
        }

    }
}
