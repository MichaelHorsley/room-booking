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
    public async Task SignUpNewHost_GivenValidData_RaisesCommand()
    {
        var request = new SignUpNewHostRequest
        {
                FirstName = "TestFirstName",
                Surname = "TestSurname",
                Email = "Test.Email@invalid"
        };

        var response = await SendApiRequest("/Host/SignUp", request);

        response.EnsureSuccessStatusCode();
        _mockCommandHandler
            .Verify(x =>
                    x.Dispatch(It.Is<SignUpNewHostCommand>(y =>
                        y.FirstName.Equals(request.FirstName)
                        && y.Email.Equals(request.Email)
                        && y.Surname.Equals(request.Surname)
                        )),
                Times.Once);
    }

    [Test]
    public async Task SignUpNewHost_GivenInvalidData_ReturnsError()
    {
        var request = new SignUpNewHostRequest
        {
            Email = "",
            FirstName = "",
            Surname = "",
        };

        var response = await SendApiRequest("/Host/SignUp", request);

        Assert.IsFalse(response.IsSuccessStatusCode);

        _mockCommandHandler.Verify(x => x.Dispatch(It.IsAny<SignUpNewHostCommand>()), Times.Never);
    }

    private async Task<HttpResponseMessage> SendApiRequest(string url, object messageBody)
    {
        using var httpClient = _testApi.CreateClient();

        return await httpClient.PostAsync(url, JsonContent.Create(messageBody));
    }
}