using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Domain.Prices.Contracts;
using Lykke.Domain.Prices.Model;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.MarketProfile.Core;
using Lykke.Service.MarketProfile.Core.Domain;
using Lykke.Service.MarketProfile.Core.Services;

namespace Lykke.Service.MarketProfile.Services
{
    public class MarketProfileManager :
        IMarketProfileManager,
        IDisposable
    {
        private readonly ILog _log;
        private readonly RabbitMqSubscriptionSettings _rabbitMqSubscriptionSettings;
        private readonly TimeSpan _cachePersistPeriod;
        private readonly IAssetPairsCacheService _cacheService;
        private readonly IAssetPairsRepository _repository;

        private RabbitMqSubscriber<IQuote> _subscriber;
        private Timer _timer;

        public MarketProfileManager(
            ILog log,
            RabbitMqSubscriptionSettings rabbitMqSubscriptionSettings,
            TimeSpan cachePersistPeriod,
            IAssetPairsCacheService cacheService,
            IAssetPairsRepository repository)
        {
            _log = log;
            _rabbitMqSubscriptionSettings = rabbitMqSubscriptionSettings;
            _cachePersistPeriod = cachePersistPeriod;
            _cacheService = cacheService;
            _repository = repository;
        }

        public void Start()
        {
            try
            {
                UpdateCache().Wait();

                _subscriber = new RabbitMqSubscriber<IQuote>(_rabbitMqSubscriptionSettings, new DefaultErrorHandlingStrategy(_log, _rabbitMqSubscriptionSettings))
                    .SetMessageDeserializer(new JsonMessageDeserializer<Quote>())
                    .SetMessageReadStrategy(new MessageReadWithTemporaryQueueStrategy())
                    .Subscribe(ProcessQuote)
                    .SetLogger(_log)
                    .Start();

                _timer = new Timer(PersistCache, null, _cachePersistPeriod, Timeout.InfiniteTimeSpan);
            }
            catch (Exception ex)
            {
                _log.WriteErrorAsync(Constants.ComponentName, null, null, ex).Wait();
                throw;
            }
        }

        private async void PersistCache(object state)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);

            try
            {
                var pairs = _cacheService.GetAll();

                await _repository.Write(pairs);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(Constants.ComponentName, null, null, ex);
            }
            finally
            {
                _timer.Change(_cachePersistPeriod, Timeout.InfiniteTimeSpan);
            }
        }

        private async Task UpdateCache()
        {
            var pairs = await _repository.Read();

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