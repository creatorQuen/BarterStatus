﻿using LeadStatusUpdater;
using LeadStatusUpdater.Common;
using LeadStatusUpdater.Models;
using LeadStatusUpdater.Services;
using LeadStatusUpdater.Settings;
using LeadStatusUpdaterTests.DataHelpers;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            ConverterService.RatesModel = ConverterServiceData.GetRatesModel();
            var _iOptionsMock = new Mock<IOptions<AppSettings>>();
            var publisher = new RabbitMqPublisher(_iOptionsMock.Object);
            _sut = new SetVipService(_requestsMock.Object, converter, publisher);
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

        [TestCase()]
        public void ProcessTests_Object_NoContentReturned()
        {
            //When
            var leadsOfFirstLoop = SetVipServiceData.GetLeadsOutputModel();
            var leadsToChangeStatusList = SetVipServiceData.GetLeadsToChangeStatusList();

            _requestsMock
                .SetupSequence(x => x.GetRegularAndVipLeads(It.IsAny<int>()))
                .Returns(leadsOfFirstLoop)
                .Returns(new List<LeadOutputModel>());
            _requestsMock
                .SetupSequence(x => x.GetTransactionsByPeriod(It.IsAny<List<int>>()))
                .Returns(SetVipServiceData.GetTransactionsByLeadId(4))
                .Returns(SetVipServiceData.GetTransactionsByLeadId(5))
                .Returns(SetVipServiceData.GetTransactionsByLeadId(6));
            _requestsMock
                .Setup(x => x.ChangeStatus(leadsToChangeStatusList));

            //Given
            _sut.Process(new object());

            //Then
        }


    }
}