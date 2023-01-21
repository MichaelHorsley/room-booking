using AutoMapper;
using commands;
using host_api.Mapping;
using host_api.Requests;
using NUnit.Framework;

namespace host_api_tests.MappingTests;

[TestFixture]
public class RegisterNewHostRequest_RegisterNewHostCommand_ProfileTests
{
    private Mapper _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = new Mapper(new MapperConfiguration(x =>
            x.AddProfile(new RegisterNewHostRequest_RegisterNewHostCommand_Profile())));
    }

    [Test]
    public void Map_Email()
    {
        var registerNewHostRequest = new RegisterNewHostRequest
        {
            Email = "testingEmail@domain.invalid"
        };

        var registerNewHostCommand = _sut.Map<RegisterNewHostCommand>(registerNewHostRequest);

        Assert.AreEqual(registerNewHostRequest.Email, registerNewHostCommand.Email);
    }

    [Test]
    public void Map_FirstName()
    {
        var registerNewHostRequest = new RegisterNewHostRequest
        {
            FirstName = Guid.NewGuid().ToString()
        };

        var registerNewHostCommand = _sut.Map<RegisterNewHostCommand>(registerNewHostRequest);

        Assert.AreEqual(registerNewHostRequest.FirstName, registerNewHostCommand.FirstName);
    }

    [Test]
    public void Map_Surname()
    {
        var registerNewHostRequest = new RegisterNewHostRequest
        {
            Surname = Guid.NewGuid().ToString()
        };

        var registerNewHostCommand = _sut.Map<RegisterNewHostCommand>(registerNewHostRequest);

        Assert.AreEqual(registerNewHostRequest.Surname, registerNewHostCommand.Surname);
    }

    [Test]
    public void Map_CorrelationId()
    {
        var registerNewHostRequest = new RegisterNewHostRequest
        {
            CorrelationId = Guid.NewGuid()
        };

        var registerNewHostCommand = _sut.Map<RegisterNewHostCommand>(registerNewHostRequest);

        Assert.AreEqual(registerNewHostRequest.CorrelationId, registerNewHostCommand.CorrelationId);
    }
}