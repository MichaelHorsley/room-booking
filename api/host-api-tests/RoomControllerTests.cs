using host_api;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace host_api_tests
{
    [TestFixture]
    internal class RoomControllerTests
    {
        [Test]
        public async Task TestFoo()
        {
            using var application = new WebApplicationFactory<Program>();

            var httpClient = application.CreateClient();

            var httpResponseMessage = await httpClient.PostAsync("/Room/RegisterNewRoom", new StringContent(""));


        }
    }
}