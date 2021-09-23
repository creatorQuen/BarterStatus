using NUnit.Framework;
using LeadStatusUpdater.Services;
using LeadStatusUpdaterTests.DataHelpers;

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
    }
}