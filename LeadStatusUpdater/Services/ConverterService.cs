using Exchange;
using System;

namespace LeadStatusUpdater.Services
{
    public class ConverterService : IConverterService
    {
        public static RatesExchangeModel RatesModel { get; set; }

        private string _baseCurrency;

        public decimal ConvertAmount(string senderCurrency, string recipientCurrency, decimal amount)
        {
            _baseCurrency = RatesModel.BaseCurrency;

            RatesModel.Rates.TryGetValue($"{_baseCurrency}{senderCurrency}", out var senderCurrencyValue);

            RatesModel.Rates.TryGetValue($"{_baseCurrency}{recipientCurrency}", out var recipientCurrencyValue);

            if (senderCurrency == _baseCurrency)
                senderCurrencyValue = 1m;

            if (recipientCurrency == _baseCurrency)
                recipientCurrencyValue = 1m;

            return Decimal.Round((senderCurrencyValue / recipientCurrencyValue * amount), 3);
        }
    }
}
