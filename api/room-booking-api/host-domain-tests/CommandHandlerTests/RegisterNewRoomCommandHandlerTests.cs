﻿using commands;
using host_domain.Aggregates;
using host_domain.CommandHandlers;
using host_domain.Services;
using Moq;
using NUnit.Framework;
using Serilog;

namespace host_domain_tests.CommandHandlerTests
{
    [TestFixture]
    public class RegisterNewRoomCommandHandlerTests
    {
        private RegisterNewRoomCommandHandler _sut;
        private Mock<IAggregateService> _mockAggregateService;

        [SetUp]
        public void SetUp()
        {
            _mockAggregateService = new Mock<IAggregateService>();

            _mockAggregateService.Setup(x => x.Get<RoomAggregate>(It.IsAny<string>())).Returns(new RoomAggregate(""));

            _sut = new RegisterNewRoomCommandHandler(new Mock<ILogger>().Object, _mockAggregateService.Object);
        }

        [Test]
        public void GivenCommand_CallsAggregateWithRightId()
        {
            var hostId = Guid.NewGuid();
            var roomId = "roomId1";
            
            _sut.Handle(new RegisterNewRoomCommand
            {
                HostId = hostId,
                RoomId = roomId
            });

            _mockAggregateService.Verify(x => x.Get<RoomAggregate>($"{hostId}|{roomId}"));
        }
    }
}