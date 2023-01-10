using AutoMapper;
using host_api.Controllers;
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
                RoomId = Guid.NewGuid()
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
    }
}