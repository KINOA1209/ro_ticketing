using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace OmnitracsIntegration.Tests
{
    public class UserManagementUnitTests
    {
        private readonly ITestOutputHelper _output;

        public UserManagementUnitTests(ITestOutputHelper output)
        {
            this._output = output;
        }

        [Fact]
        public async Task TestGetCompanyDetails()
        {
            var response = await OmnitracsUserManagement.Instance.GetCompanyDetailsAsync();
            _output.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
        }
    }
}
