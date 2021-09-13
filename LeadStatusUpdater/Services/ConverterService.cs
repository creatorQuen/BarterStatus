using LeadStatusUpdater.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadStatusUpdater.Services
{
    public class ConverterService : IConverterService
    {
        private readonly string _baseCurrency;
        private readonly RatesExchangeModel _ratesModel;
        public ConverterService()
        {
        }

        public decimal ConvertAmount(string senderCurrency, string recipientCurrency, decimal amount)
        {
            if (!IsValid(senderCurrency) || !IsValid(recipientCurrency)) throw new Exception("Currency is not valid");

            _ratesModel.Rates.TryGetValue($"{_baseCurrency}{senderCurrency}", out var senderCurrencyValue);

            _ratesModel.Rates.TryGetValue($"{_baseCurrency}{recipientCurrency}", out var recipientCurrencyValue);

            if (senderCurrency == _baseCurrency)
                senderCurrencyValue = 1m;

            if (recipientCurrency == _baseCurrency)
                recipientCurrencyValue = 1m;

            return Decimal.Round((senderCurrencyValue / recipientCurrencyValue * amount), 3);
        }

        private bool IsValid(string currency)
        {
            if (currency == _baseCurrency)
                return true;
            return _ratesModel.Rates.ContainsKey($"{_baseCurrency}{currency}");
        }
    }
}
