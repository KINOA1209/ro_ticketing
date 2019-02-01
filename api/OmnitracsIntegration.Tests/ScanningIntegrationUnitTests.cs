using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace OmnitracsIntegration.Tests
{
    public class ScanningIntegrationUnitTests
    {
        private readonly ITestOutputHelper _output;

        public ScanningIntegrationUnitTests(ITestOutputHelper output)
        {
            this._output = output;
        }

        [Fact]
        public async Task TestGetMessageCount()
        {
            var response = await OmnitracsScanningIntegration.Instance.GetMessageCountAsync();
            _output.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
        }
    }
}
