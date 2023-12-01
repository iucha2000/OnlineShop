using Application.Services;
using Application.Services.Models;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ExchangeRate : IExchangeRate
    {
        private readonly ICacheService _cacheService;
        private readonly ExchangeRatesConfigModel _config;

        public ExchangeRate(ICacheService cacheService, IOptions<ExchangeRatesConfigModel> config)
        {
            _cacheService = cacheService;
            _config = config.Value;
        }

        public async Task<ExchangeRatesModel> GetExchangeRates(string currency)
        {
            var data = _cacheService.GetData<ExchangeRatesModel>(currency);
            if(data == null)
            {
                var url = _config.BaseUrl + _config.ApiKey + _config.Param + currency;

                data = await url.GetJsonAsync<ExchangeRatesModel>();

                _cacheService.SetData(currency, data);
            }

            return data;
        }
    }
}
