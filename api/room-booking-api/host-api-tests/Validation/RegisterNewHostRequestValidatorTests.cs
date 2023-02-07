using host_api.Requests;
using host_api.Validation;
using NUnit.Framework;

namespace host_api_tests.Validation;

[TestFixture]
public class RegisterNewHostRequestValidatorTests
{
    private SignUpNewHostRequestValidator _sut;
    private SignUpNewHostRequest _request;

    [SetUp]
    public void SetUp()
    {
        _sut = new SignUpNewHostRequestValidator();

        _request = new SignUpNewHostRequest
        {
            FirstName = "Test-First-Name",
            Surname = "Test-Surname",
            Email = "testemail@invalid",
        };
    }

    [Test]
    public void Validate_GivenRequestWithDefaultFirstname_ReturnsFalse()
    {
        _request.FirstName = string.Empty;

        var validationResult = _sut.Validate(_request);

        Assert.IsFalse(validationResult.IsValid);
        Assert.AreEqual("'First Name' must not be empty.", validationResult.Errors.First().ErrorMessage);
    }

    [Test]
    public void Validate_GivenRequestWithDefaultSurname_ReturnsFalse()
    {
        _request.Surname = string.Empty;

        var validationResult = _sut.Validate(_request);

        Assert.IsFalse(validationResult.IsValid);
        Assert.AreEqual("'Surname' must not be empty.", validationResult.Errors.First().ErrorMessage);
    }

    [Test]
    public void Validate_GivenRequestWithDefaultEmail_ReturnsFalse()
    {
        _request.Email = string.Empty;

        var validationResult = _sut.Validate(_request);

        Assert.IsFalse(validationResult.IsValid);
        Assert.AreEqual("'Email' must not be empty.", validationResult.Errors.First().ErrorMessage);
    }
}