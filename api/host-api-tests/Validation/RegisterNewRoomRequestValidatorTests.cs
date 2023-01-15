using host_api.Requests;
using host_api.Validation;
using NUnit.Framework;

namespace host_api_tests.Validation
{
    [TestFixture]
    public class RegisterNewRoomRequestValidatorTests
    {
        private RegisterNewRoomRequestValidator _sut;
        private RegisterNewRoomRequest _request;

        [SetUp]
        public void SetUp()
        {
            _sut = new RegisterNewRoomRequestValidator();

            _request = new RegisterNewRoomRequest
            {
                HostId = Guid.NewGuid(),
                RoomId = "roomid-1"
            };
        }

        [Test]
        public void Validate_GivenRequestWithDefaultHostId_ReturnsFalse()
        {
            _request.HostId = Guid.Empty;

            var validationResult = _sut.Validate(_request);

            Assert.IsFalse(validationResult.IsValid);
            Assert.AreEqual("'Host Id' must not be empty.", validationResult.Errors.First().ErrorMessage);
        }

        [Test]
        public void Validate_GivenRequestWithDefaultRoomId_ReturnsFalse()
        {
            _request.RoomId = "";

            var validationResult = _sut.Validate(_request);

            Assert.IsFalse(validationResult.IsValid);
            Assert.AreEqual("'Room Id' must not be empty.", validationResult.Errors.First().ErrorMessage);
        }
    }
}