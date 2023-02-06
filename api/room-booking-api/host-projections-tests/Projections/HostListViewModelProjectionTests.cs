using events;
using host_projections.Projections;
using host_projections.Repositories;
using Moq;
using NUnit.Framework;
using view_models;

namespace host_projections_tests.Projections
{
    [TestFixture]
    public class HostListViewModelProjectionTests
    {
        private HostListViewModelProjection _sut;

        private Mock<IViewModelRepository> _mockRepository;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new Mock<IViewModelRepository>();
            _sut = new HostListViewModelProjection(_mockRepository.Object);
        }

        [Test]
        public void SavesObjectWithCorrectId()
        {
            var hostRegisteredEvent = new HostSignedUpEvent
            {
                AggregateId = Guid.NewGuid().ToString()
            };

            _sut.Handle(hostRegisteredEvent);

            _mockRepository.Verify(x => x.Save(It.Is<AllHostsViewModel>(y => y.Id.Equals(hostRegisteredEvent.AggregateId))));
        }

        [Test]
        public void SavesObjectWithCorrectFirstName()
        {
            var hostRegisteredEvent = new HostSignedUpEvent
            {
                FirstName = Guid.NewGuid().ToString()
            };

            _sut.Handle(hostRegisteredEvent);

            _mockRepository.Verify(x => x.Save(It.Is<AllHostsViewModel>(y => y.FirstName.Equals(hostRegisteredEvent.FirstName))));
        }

        [Test]
        public void SavesObjectWithCorrectSurname()
        {
            var hostRegisteredEvent = new HostSignedUpEvent
            {
                Surname = Guid.NewGuid().ToString()
            };

            _sut.Handle(hostRegisteredEvent);

            _mockRepository.Verify(x => x.Save(It.Is<AllHostsViewModel>(y => y.Surname.Equals(hostRegisteredEvent.Surname))));
        }

        [Test]
        public void SavesObjectWithCorrectEmail()
        {
            var hostRegisteredEvent = new HostSignedUpEvent
            {
                Email = Guid.NewGuid().ToString()
            };

            _sut.Handle(hostRegisteredEvent);

            _mockRepository.Verify(x => x.Save(It.Is<AllHostsViewModel>(y => y.Email.Equals(hostRegisteredEvent.Email))));
        }
    }
}