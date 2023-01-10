using System.Net.Http.Json;
using host_api.Controllers;
using host_api.Requests;
using host_api.Services;
using Moq;
using NUnit.Framework;

namespace host_api_tests.Controllers
{
    [TestFixture]
    internal class RoomControllerTests
    {
        private TestingApplication _testApi;
        private Mock<ICommandHandler> _mockCommandHandler;

        [SetUp]
        public void SetUp()
        {
            _testApi = new TestingApplication();

            _mockCommandHandler = new Mock<ICommandHandler>();

            _testApi.AddServiceToDependencyInjection<ICommandHandler>(_mockCommandHandler.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _testApi?.Dispose();
        }

        [Test]
        public async Task RegisterNewRoom_GivenValidData_RaisesCommand()
        {
            var roomId = Guid.NewGuid();
            var hostId = Guid.NewGuid();

            var request = new RegisterNewRoomRequest
            {
                RoomId = roomId,
                HostId = hostId
            };

            var response = await SendApiRequest("/Room/RegisterNewRoom", request);

            response.EnsureSuccessStatusCode();
            _mockCommandHandler
                .Verify(x => 
                    x.Dispatch(It.Is<RegisterNewRoomCommand>(y => 
                        y.HostId.Equals(hostId) 
                        && y.RoomId.Equals(roomId) 
                        && !y.Id.Equals(Guid.Empty))), 
                    Times.Once);
        }

        [Test]
        public async Task RegisterNewRoom_GivenInvalidData_ReturnsError()
        {
            var request = new RegisterNewRoomRequest
            {
                RoomId = Guid.Empty,
                HostId = Guid.Empty
            };

            var response = await SendApiRequest("/Room/RegisterNewRoom", request);

            Assert.IsFalse(response.IsSuccessStatusCode);

            _mockCommandHandler.Verify(x => x.Dispatch(It.IsAny<RegisterNewRoomCommand>()), Times.Never);
        }

        private async Task<HttpResponseMessage> SendApiRequest(string url, object messageBody)
        {
            using var httpClient = _testApi.CreateClient();

            return await httpClient.PostAsync(url, JsonContent.Create(messageBody));
        }
    }
}