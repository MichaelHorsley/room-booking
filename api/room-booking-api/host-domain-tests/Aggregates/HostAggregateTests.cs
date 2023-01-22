using events;
using host_domain;
using host_domain.Aggregates;
using host_domain.Repositories;
using host_domain.Services;
using Moq;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace host_domain_tests.Aggregates;

[TestFixture]
public class HostAggregateTests
{
    private HostAggregate _sut;
    private Mock<IEventRepository> _mockEventRepository;
    private Mock<IEventDispatcher> _mockEventDispatcher;

    [SetUp]
    public void SetUp()
    {
        _mockEventRepository = new Mock<IEventRepository>();
        _mockEventDispatcher = new Mock<IEventDispatcher>();

        _sut = new HostAggregate("aggregateId", _mockEventRepository.Object, _mockEventDispatcher.Object);

    }

    [Test]
    public void GivenHostNotAlreadyCreated_RaisesEvent()
    {
        var expectedEventRaised = new HostRegisteredEvent
        {
            Email = "email@testing.invalid",
            FirstName = "TestFirstName",
            Surname = "TestSurname"
        };

        var cleansedString = Regex.Replace((expectedEventRaised.FirstName + expectedEventRaised.Surname + expectedEventRaised.Email).ToLower(), "[^0-9a-zA-Z]+", "");
        var aggregateGuid = GuidUtility.Create(cleansedString);
        
        _sut = new HostAggregate(aggregateGuid.ToString(), _mockEventRepository.Object, _mockEventDispatcher.Object);

        _sut.RegisterNewHost(expectedEventRaised.Email, expectedEventRaised.FirstName, expectedEventRaised.Surname);

        _mockEventRepository
            .Verify(x =>
                x.SaveEvent(It.Is<HostRegisteredEvent>(y =>
                    y.Email.Equals(expectedEventRaised.Email)
                    && y.FirstName.Equals(expectedEventRaised.FirstName)
                    && y.Surname.Equals(expectedEventRaised.Surname)
                    && y.AggregateId.Equals(aggregateGuid.ToString())
                )), Times.Once);
    }

    [Test]
    public void GivenHostAlreadyCreated_DoesNotRaisesEvent()
    {
        var registeredEvent = new HostRegisteredEvent
        {
            Email = "email@testing.invalid",
            FirstName = "TestFirstName",
            Surname = "TestSurname"
        };

        _sut.ApplyEventAgainstAggregate(registeredEvent);

        _sut.RegisterNewHost(registeredEvent.Email, registeredEvent.FirstName, registeredEvent.Surname);

        _mockEventRepository
            .Verify(x =>
                x.SaveEvent(It.IsAny<HostRegisteredEvent>()), Times.Never);
    }
}