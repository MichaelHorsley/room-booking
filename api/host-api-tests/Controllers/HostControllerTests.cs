using System.Net.Http.Json;
using commands;
using host_api.Requests;
using host_api.Services;
using Moq;
using NUnit.Framework;

namespace host_api_tests.Controllers;

[TestFixture]
internal class HostControllerTests
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
    public async Task RegisterNewHost_GivenValidData_RaisesCommand()
    {
        var request = new RegisterNewHostRequest
        {
                FirstName = "TestFirstName",
                Surname = "TestSurname",
                Email = "Test.Email@invalid"
        };

        var response = await SendApiRequest("/Host/RegisterNewHost", request);

        response.EnsureSuccessStatusCode();
        _mockCommandHandler
            .Verify(x =>
                    x.Dispatch(It.Is<RegisterNewHostCommand>(y =>
                        y.FirstName.Equals(request.FirstName)
                        && y.Email.Equals(request.Email)
                        && y.Surname.Equals(request.Surname)
                        )),
                Times.Once);
    }

    [Test]
    public async Task RegisterNewHost_GivenInvalidData_ReturnsError()
    {
        var request = new RegisterNewHostRequest
        {
            Email = "",
            FirstName = "",
            Surname = "",
        };

        var response = await SendApiRequest("/Host/RegisterNewHost", request);

        Assert.IsFalse(response.IsSuccessStatusCode);

        _mockCommandHandler.Verify(x => x.Dispatch(It.IsAny<RegisterNewHostCommand>()), Times.Never);
    }

    private async Task<HttpResponseMessage> SendApiRequest(string url, object messageBody)
    {
        using var httpClient = _testApi.CreateClient();

        return await httpClient.PostAsync(url, JsonContent.Create(messageBody));
    }
}