using OmnitracsIntegration.SoapSecurity;
using System;
using System.Threading.Tasks;
using EIPOutMsgSvcsReference;
using TMMWebSvcsReference;

namespace OmnitracsIntegration
{
    public class OmnitracsTextAndMacroIntegration
    {
        private static OmnitracsTextAndMacroIntegration _instance;
        public static OmnitracsTextAndMacroIntegration Instance => _instance ?? (_instance = new OmnitracsTextAndMacroIntegration());

        public async Task<createMacroResponse> CreateMacroAsync()
        {
            var factory = SoapSecurityHelper.CreateSecuredChannel<TMMWebSvcs>(OmnitracsConfiguration.TMMWebSvcsEndpoint, OmnitracsConfiguration.Username, OmnitracsConfiguration.Password);
            var serviceProxy = factory.CreateChannel();

            var request = new createMacroRequest();
            createMacroResponse response;
            try
            {
                response = await serviceProxy.createMacroAsync(request);
            }
            catch (Exception ex)
            {
                throw;
            }

            factory.Close();

            return response;
        }

        public async Task<getMacroListResponse> GetMacroListAsync()
        {
            var factory = SoapSecurityHelper.CreateSecuredChannel<TMMWebSvcs>(OmnitracsConfiguration.TMMWebSvcsEndpoint, OmnitracsConfiguration.Username, OmnitracsConfiguration.Password);
            var serviceProxy = factory.CreateChannel();

            var request = new getMacroListRequest();
            request.direction = "A";
            getMacroListResponse response;
            try
            {
                response = await serviceProxy.getMacroListAsync(request);
            }
            catch (Exception ex)
            {
                throw;
            }

            factory.Close();

            return response;
        }

        public async Task<sendTextMessageResponse> SendTextMessageAsync()
        {
            var factory = SoapSecurityHelper.CreateSecuredChannel<TMMWebSvcs>(OmnitracsConfiguration.TMMWebSvcsEndpoint, OmnitracsConfiguration.Username, OmnitracsConfiguration.Password);
            var serviceProxy = factory.CreateChannel();

            var request = new sendTextMessageRequest();
            request.addressees = "TREVOR";
            request.sender = "WEBSVC";
            request.macro = 2;
            request.body = "<macroBody><macroField dictTag='tstMonetary'>8.27</macroField><macroField dictTag='tstAlphabetic'>abc</macroField><macroField dictTag='tstRealNumber'>3.14</macroField><macroField dictTag='tstNumeric'>12</macroField></macroBody>";
            //request.macro = 0;
            //request.body = "<msgBody>Hello Trevor!</msgBody>";
            request.priority = 1;
            request.replyTo = "*Personal";
            request.returnReceipt = true;
            sendTextMessageResponse response;
            try
            {
                response = await serviceProxy.sendTextMessageAsync(request);
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
