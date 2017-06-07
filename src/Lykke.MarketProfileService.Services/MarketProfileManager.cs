using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Domain.Prices.Contracts;
using Lykke.Domain.Prices.Model;
using Lykke.MarketProfileService.Core;
using Lykke.MarketProfileService.Core.Domain;
using Lykke.MarketProfileService.Core.Services;
using Lykke.MarketProfileService.Services.RabbitMq;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.MarketProfileService.Services
{
    public class MarketProfileManager :
        IMarketProfileManager,
        IDisposable
    {
        private readonly ILog _log;
        private readonly ApplicationSettings.MarketProfileServiceSettings _settings;
        private readonly IAssetPairsCacheService _cacheService;
        private readonly IAssetPairsRepository _repository;

        private RabbitMqSubscriber<IQuote> _subscriber;
        private Timer _timer;

        public MarketProfileManager(
            ILog log,
            ApplicationSettings.MarketProfileServiceSettings settings,
            IAssetPairsCacheService cacheService,
            IAssetPairsRepository repository)
        {
            _log = log;
            _settings = settings;
            _cacheService = cacheService;
            _repository = repository;
        }

        public void Start()
        {
            try
            {
                UpdateCache().Wait();

                _subscriber = new RabbitMqSubscriber<IQuote>(new RabbitMqSubscriberSettings
                    {
                        ConnectionString = _settings.QuoteFeedRabbitSettings.ConnectionString,
                        QueueName = $"{_settings.QuoteFeedRabbitSettings.ExchangeName}.marketprofileservice",
                        ExchangeName = _settings.QuoteFeedRabbitSettings.ExchangeName
                    })
                    .SetMessageDeserializer(new JsonMessageDeserializer<Quote>())
                    .SetMessageReadStrategy(new MessageReadWithTransientAutodeleteQueueStrategy())
                    .Subscribe(ProcessQuote)
                    .SetLogger(_log)
                    .Start();

                _timer = new Timer(PersistCache, null, _settings.CacheSettings.PersistPeriod,
                    _settings.CacheSettings.PersistPeriod);
            }
            catch (Exception ex)
            {
                _log.WriteErrorAsync(Constants.ComponentName, null, null, ex).Wait();
            }
        }

        private async void PersistCache(object state)
        {
            try
            {
                var pairs = _cacheService.GetAll();

                await _repository.AddOrUpdateAllAsync(pairs);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(Constants.ComponentName, null, null, ex);
            }
        }

        private async Task UpdateCache()
        {
            var pairs = await _repository.GetAllAsync();

            _cacheService.InitCache(pairs);
        }

        private async Task ProcessQuote(IQuote entry)
        {
            try
            {
                _cacheService.UpdatePair(entry);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(Constants.ComponentName, null, null, ex);
            }
        }

        public IAssetPair TryGetPair(string pairCode)
        {
            return _cacheService.TryGetPair(pairCode);
        }

        public IAssetPair[] GetAllPairs()
        {
            return _cacheService.GetAll();
        }

        public void Dispose()
        {
            _timer.Dispose();
            _subscriber.Stop();
        }
    }
}