using AutoMapper;
using commands;
using host_api.Mapping;
using host_api.Requests;
using NUnit.Framework;

namespace host_api_tests.MappingTests
{
    [TestFixture]
    public class RegisterNewRoomRequest_RegisterNewRoomCommand_ProfileTests
    {
        private Mapper _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new Mapper(new MapperConfiguration(x =>
                x.AddProfile(new RegisterNewRoomRequest_RegisterNewRoomCommand_Profile())));
        }

        [Test]
        public void Map_RoomId()
        {
            var registerNewRoomRequest = new RegisterNewRoomRequest
            {
                RoomId = "room-id-1"
            };

            var registerNewRoomCommand = _sut.Map<RegisterNewRoomCommand>(registerNewRoomRequest);

            Assert.AreEqual(registerNewRoomRequest.RoomId, registerNewRoomCommand.RoomId);
        }

        [Test]
        public void Map_HostId()
        {
            var registerNewRoomRequest = new RegisterNewRoomRequest
            {
                HostId = Guid.NewGuid()
            };

            var registerNewRoomCommand = _sut.Map<RegisterNewRoomCommand>(registerNewRoomRequest);

            Assert.AreEqual(registerNewRoomRequest.HostId, registerNewRoomCommand.HostId);
        }

        [Test]
        public void Map_CorrelationId()
        {
            var registerNewRoomRequest = new RegisterNewRoomRequest
            {
                CorrelationId = Guid.NewGuid()
            };

            var registerNewRoomCommand = _sut.Map<RegisterNewRoomCommand>(registerNewRoomRequest);

            Assert.AreEqual(registerNewRoomRequest.CorrelationId, registerNewRoomCommand.CorrelationId);
        }
    }
}