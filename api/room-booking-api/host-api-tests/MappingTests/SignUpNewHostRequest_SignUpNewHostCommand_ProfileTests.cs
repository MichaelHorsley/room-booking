using AutoMapper;
using commands;
using host_api.Mapping;
using host_api.Requests;
using NUnit.Framework;

namespace host_api_tests.MappingTests;

[TestFixture]
public class SignUpNewHostRequest_SignUpNewHostCommand_ProfileTests
{
    private Mapper _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = new Mapper(new MapperConfiguration(x =>
            x.AddProfile(new SignUpNewHostRequest_SignUpNewHostCommand_Profile())));
    }

    [Test]
    public void Map_Email()
    {
        var signUpNewHostRequest = new SignUpNewHostRequest
        {
            Email = "testingEmail@domain.invalid"
        };

        var registerNewHostCommand = _sut.Map<SignUpNewHostCommand>(signUpNewHostRequest);

        Assert.AreEqual(signUpNewHostRequest.Email, registerNewHostCommand.Email);
    }

    [Test]
    public void Map_FirstName()
    {
        var signUpNewHostRequest = new SignUpNewHostRequest
        {
            FirstName = Guid.NewGuid().ToString()
        };

        var registerNewHostCommand = _sut.Map<SignUpNewHostCommand>(signUpNewHostRequest);

        Assert.AreEqual(signUpNewHostRequest.FirstName, registerNewHostCommand.FirstName);
    }

    [Test]
    public void Map_Surname()
    {
        var signUpNewHostRequest = new SignUpNewHostRequest
        {
            Surname = Guid.NewGuid().ToString()
        };

        var registerNewHostCommand = _sut.Map<SignUpNewHostCommand>(signUpNewHostRequest);

        Assert.AreEqual(signUpNewHostRequest.Surname, registerNewHostCommand.Surname);
    }

    [Test]
    public void Map_CorrelationId()
    {
        var signUpNewHostRequest = new SignUpNewHostRequest
        {
            CorrelationId = Guid.NewGuid()
        };

        var registerNewHostCommand = _sut.Map<SignUpNewHostCommand>(signUpNewHostRequest);

        Assert.AreEqual(signUpNewHostRequest.CorrelationId, registerNewHostCommand.CorrelationId);
    }
}