using LeadStatusUpdater;
using LeadStatusUpdater.Common;
using LeadStatusUpdater.Models;
using LeadStatusUpdater.Services;
using LeadStatusUpdaterTests.DataHelpers;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeadStatusUpdaterTests
{
    public class SetVipServiceTests
    {
        private Mock<IRequestsSender> _requestsMock = new Mock<IRequestsSender>();
        private Mock<IRabbitMqPublisher> _rabbitMqPublisherMock = new Mock<IRabbitMqPublisher>();
        
        private SetVipService _sut;

        [SetUp]
        public void Setup()
        {
            var converter = new ConverterService();
            ConverterService.RatesModel = ConverterServiceData.GetRatesModel();
            _sut = new SetVipService(_requestsMock.Object, converter, _rabbitMqPublisherMock.Object);
        }

        [TestCaseSource(typeof(SetVipServiceData), nameof(SetVipServiceData.GetDataForCheckLead))]
        public async Task CheckOneLeadTests_LeadOutputModel_BooleanReturned(LeadOutputModel lead, List<int> accountIds, 
            List<TransactionOutputModel> transactions, bool expected)
        {
            //When
            _requestsMock.Setup(x => x.GetTransactionsByPeriod(It.IsAny<List<int>>())).Returns(transactions);

            //Given
            var actual = await _sut.CheckOneLead(lead);

            //Then
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ProcessTests_Object_NoContentReturned()
        {
            //When
            var leadsOfFirstLoop = SetVipServiceData.GetLeadsOutputModel();
            var leadsToChangeStatusList = SetVipServiceData.GetLeadsToChangeStatusList();
            var accountIds= new List<int> { 4, 5, 6};

            _requestsMock.Setup(x => x.GetAdminToken()).Returns(It.IsAny<string>());
            _requestsMock
                .SetupSequence(x => x.GetRegularAndVipLeads(It.IsAny<int>()))
                .Returns(leadsOfFirstLoop)
                .Returns(new List<LeadOutputModel>());
            accountIds.ForEach( accountId => _requestsMock.Setup(x => x.GetTransactionsByPeriod(new List<int> { accountId}))
                .Returns(SetVipServiceData.GetTransactionsByLeadId(accountId)));
            _requestsMock
                .Setup(x => x.ChangeStatus(leadsToChangeStatusList));

            //Given
            _sut.Process(new object());

            //Then
            _requestsMock.Verify(x => x.GetAdminToken(), Times.Once);
            _requestsMock.Verify(x => x.GetRegularAndVipLeads(It.IsAny<int>()), Times.Exactly(2));
            _requestsMock.Verify(x => x.GetRegularAndVipLeads(It.IsAny<int>()), Times.Exactly(2));
            accountIds.ForEach(accountId => _requestsMock.Verify(x => x.GetTransactionsByPeriod(new List<int> { accountId }), Times.Once));
            _requestsMock.Verify(x => x.ChangeStatus(leadsToChangeStatusList), Times.AtMostOnce());
        }


    }
}
