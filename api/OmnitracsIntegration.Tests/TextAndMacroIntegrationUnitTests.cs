using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace OmnitracsIntegration.Tests
{
    public class TextAndMacroIntegrationUnitTests
    {
        private readonly ITestOutputHelper _output;

        public TextAndMacroIntegrationUnitTests(ITestOutputHelper output)
        {
            this._output = output;
        }

        [Fact]
        public async Task TestCreateMacroAsync()
        {
            //var response = await OmnitracsTextAndMacroIntegration.Instance.CreateMacroAsync();
            //_output.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
        }

        [Fact]
        public async Task TestGetMacroListAsync()
        {
            var response = await OmnitracsTextAndMacroIntegration.Instance.GetMacroListAsync();
            _output.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
        }

        [Fact]
        public async Task TestSendTextMessageAsync()
        {
            var response = await OmnitracsTextAndMacroIntegration.Instance.SendTextMessageAsync();
            _output.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
        }

    }
}
