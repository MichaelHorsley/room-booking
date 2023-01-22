using commands;
using host_domain;
using host_domain.Aggregates;
using host_domain.CommandHandlers;
using host_domain.Repositories;
using host_domain.Services;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using Serilog;
using System.Text.RegularExpressions;

namespace host_domain_tests.CommandHandlers;

[TestFixture]
public class RegisterNewHostCommandHandlerTests
{
    private RegisterNewHostCommandHandler _sut;
    private Mock<IAggregateService> _mockAggregateService;
    private Mock<IEventRepository> _mockEventRepository;
    private Mock<IEventDispatcher> _mockEventDispatcher;

    [SetUp]
    public void SetUp()
    {
        _mockAggregateService = new Mock<IAggregateService>();
        _mockEventRepository = new Mock<IEventRepository>();
        _mockEventDispatcher = new Mock<IEventDispatcher>();

        _mockAggregateService
            .Setup(x => x.Get<HostAggregate>(It.IsAny<string>()))
            .Returns(new HostAggregate("", _mockEventRepository.Object, _mockEventDispatcher.Object));

        _sut = new RegisterNewHostCommandHandler(new Mock<ILogger>().Object, _mockAggregateService.Object);
    }

    [Test]
    public void GivenCommand_CallsAggregateWithRightId()
    {
        var command = new RegisterNewHostCommand
        {
            FirstName = "TestName",
            Email = "TestEmail",
            Surname = "TestSurname"
        };

        var aggregateId = Regex.Replace((command.FirstName + command.Surname + command.Email).ToLower(), "[^0-9a-zA-Z]+", "");
        
        var aggregateGuid = GuidUtility.Create(aggregateId);
        
        _sut.Handle(command);

        _mockAggregateService.Verify(x => x.Get<HostAggregate>(aggregateGuid.ToString()));
    }
}