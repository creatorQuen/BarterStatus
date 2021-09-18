using NUnit.Framework;
using LeadStatusUpdater.Enums;
using System.Collections.Generic;
using LeadStatusUpdater.Services;
using LeadStatusUpdaterTests.DataHelpers;
using System;
using LeadStatusUpdater.Constants;

namespace LeadStatusUpdaterTests
{
    public class ConverterServiceTests
    {
        private ConverterService _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new ConverterService();
        }

        [TestCaseSource(typeof(ConverterServiceData), nameof(ConverterServiceData.GetDataForConvertAmount))]
        public void ConvertAmount_Currencies_RecipientAmount(string senderCurrency, string recipientCurrency, decimal amount, decimal expected)
        {
            //Given
            ConverterService.RatesModel = ConverterServiceData.GetRatesModel();

            //When
            var actual = _sut.ConvertAmount(senderCurrency, recipientCurrency, amount);

            //Then
            Assert.AreEqual(expected, actual);
        }

        [TestCaseSource(typeof(ConverterServiceData), nameof(ConverterServiceData.GetDataForConvertAmountWhenRatesModelIsNull))]
        public void ConvertAmount_RatesModelIsNull_Exception(string senderCurrency, string recipientCurrency, decimal amount)
        {
            //Given
            var expectedMessage = new Exception(LogMessages.RatesNotProvided);

            //When
            var actual = Assert.Throws<Exception>(() =>
                            _sut.ConvertAmount(senderCurrency, recipientCurrency, amount));

            //Then
            Assert.AreEqual(expectedMessage, actual.Message);
        }
    }
}