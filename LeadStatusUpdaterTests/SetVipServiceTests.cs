using LeadStatusUpdater;
using LeadStatusUpdater.Common;
using LeadStatusUpdater.Models;
using LeadStatusUpdater.Services;
using Moq;
using NUnit.Framework;

namespace LeadStatusUpdaterTests
{
    public class SetVipServiceTests
    {
        private Mock<IRequestsSender> _requestsMock = new Mock<IRequestsSender>();
        private SetVipService _sut;

        [SetUp]
        public void Setup()
        {
            var converter = new ConverterService();
            var publisher = new RabbitMqPublisher();
            _sut = new SetVipService(_requestsMock.Object, converter, publisher);
        }

        [Test]
        public void CheckOneLeadTests(LeadOutputModel lead)
        {

            Assert.Pass();
        }
    }
}
