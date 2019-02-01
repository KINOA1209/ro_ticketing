using OMASServiceReference;
using OmnitracsIntegration.SoapSecurity;
using System;
using System.Threading.Tasks;

namespace OmnitracsIntegration
{
    public class OmnitracsUserManagement
    {
        private static OmnitracsUserManagement _instance;
        public static OmnitracsUserManagement Instance => _instance ?? (_instance = new OmnitracsUserManagement());

        public async Task<getCompanyDetailsResponse> GetCompanyDetailsAsync()
        {
            var factory = SoapSecurityHelper.CreateSecuredChannel<OMASWebSvcs>(OmnitracsConfiguration.OMASWebSvcsEndpoint, OmnitracsConfiguration.Username, OmnitracsConfiguration.Password);
            var serviceProxy = factory.CreateChannel();

            var request = new getCompanyDetailsRequest { companyId = OmnitracsConfiguration.CompanyId };
            getCompanyDetailsResponse response;
            try
            {
                response = await serviceProxy.getCompanyDetailsAsync(request);
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
