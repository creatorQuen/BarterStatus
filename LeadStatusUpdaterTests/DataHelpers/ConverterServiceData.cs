using Exchange;
using LeadStatusUpdater.Enums;
using System;
using System.Collections;
using System.Collections.Generic;

namespace LeadStatusUpdaterTests.DataHelpers
{
    public static class ConverterServiceData
    {
        public static IEnumerable GetDataForConvertAmount()
        {
            yield return new object[] { Currency.USD.ToString(), Currency.RUB.ToString(), 1234m, 89140.705m};
            yield return new object[] { Currency.EUR.ToString(), Currency.RUB.ToString(), -1234m, -105256.532m};
            yield return new object[] { Currency.JPY.ToString(), Currency.RUB.ToString(), 54.576m, 36.049m};
            yield return new object[] { Currency.RUB.ToString(), Currency.RUB.ToString(), 1234m, 1234m};
            yield return new object[] { Currency.RUB.ToString(), Currency.RUB.ToString(), 0m, 0m};
        }

        public static RatesExchangeModel GetRatesModel()
        {
            return new RatesExchangeModel
            {
                Updated = DateTime.Now.ToString(),
                BaseCurrency = Currency.USD.ToString(),
                Rates = new Dictionary<string, decimal>()
                {
                    { "USDRUB", 72.2372m },
                    { "USDEUR", 0.84689m},
                    { "USDJPY", 109.364m}
                }
            };
        }

        public static IEnumerable GetDataForConvertAmountWhenRatesModelIsNull()
        {
            yield return new object[] { Currency.USD.ToString(), Currency.RUB.ToString(), 1234m };
        }
    }
}
