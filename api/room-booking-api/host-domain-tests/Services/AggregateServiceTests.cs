using host_domain.Aggregates;
using host_domain.Services;
using NUnit.Framework;

namespace host_domain_tests.Services
{
    [TestFixture]
    public class AggregateServiceTests
    {
        private AggregateService _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new AggregateService();
        }

        [Test]
        public void Get_ReturnsAggregateWithId()
        {
            var roomAggregate = _sut.Get<RoomAggregate>("1");

            Assert.AreEqual("1", roomAggregate.Id);
        }
    }
}