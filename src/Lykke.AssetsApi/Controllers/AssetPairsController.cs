using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.AssetsApi.Core.Domain.Assets;
using Lykke.AssetsApi.Core.Domain.Feed;
using Lykke.AssetsApi.Core.Services;
using Lykke.AssetsApi.Exceptions;
using Lykke.AssetsApi.Models;
using Lykke.AssetsApi.Models.AssetPairs;
using Lykke.Domain.Prices.Contracts;
using Lykke.Domain.Prices.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.AssetsApi.Controllers
{
    [Route("api/[controller]")]
    public class AssetPairsController : Controller
    {
        private readonly CachedDataDictionary<string, IAssetPair> _assetPairDictionary;
        private readonly ICandleHistoryRepository _feedCandlesRepository;
        private readonly IFeedHistoryRepository _feedHistoryRepository;
        private readonly IMarketProfileService _marketProfileService;

        public AssetPairsController(
            CachedDataDictionary<string, IAssetPair> assetPairDictionary,
            ICandleHistoryRepository feedCandlesRepository, 
            IFeedHistoryRepository feedHistoryRepository,
            IMarketProfileService marketProfileService)
        {
            _assetPairDictionary = assetPairDictionary;
            _feedCandlesRepository = feedCandlesRepository;
            _feedHistoryRepository = feedHistoryRepository;
            _marketProfileService = marketProfileService;
        }

        /// <summary>
        /// Get all asset pairs rates
        /// </summary>
        [HttpGet("rate")]
        public async Task<IEnumerable<AssetPairRateModel>> GetRate()
        {
            var assetPairsIds = (await _assetPairDictionary.Values()).Where(x => !x.IsDisabled).Select(x => x.Id);

            var marketProfile = await _marketProfileService.GetMarketProfileAsync();
            marketProfile.Profile = marketProfile.Profile.Where(x => assetPairsIds.Contains(x.Asset));

            return marketProfile.ToApiModel();
        }

        /// <summary>
        /// Get rates for asset pair
        /// </summary>
        [HttpGet("rate/{assetPairId}")]
        public async Task<AssetPairRateModel> GetRate(string assetPairId)
        {
            return (await _marketProfileService.GetFeedDataAsync(assetPairId))?.ToApiModel();
        }

        /// <summary>
        /// Get asset pairs dictionary
        /// </summary>
        [HttpGet("dictionary")]
        public async Task<IEnumerable<AssetPairModel>> GetDictionary()
        {
            var pairs = (await _assetPairDictionary.Values()).Where(x => !x.IsDisabled);

            return pairs.ToApiModel();
        }

        /// <summary>
        /// Get rates for specified period
        /// </summary>
        /// <remarks>
        /// Available period values
        ///  
        ///     "Sec",
        ///     "Minute",
        ///     "Hour",
        ///     "Day",
        ///     "Month",
        /// 
        /// </remarks>
        [HttpPost("rate/history")]
        [ProducesResponseType(typeof(IEnumerable<AssetPairRateModel>), 200)]
        [ProducesResponseType(typeof(ApiError), 400)]
        public async Task<IActionResult> GetHistoryRate([FromBody] AssetPairsRateHistoryRequest request)
        {
            if (request.Period != Period.Day)
            {
                return BadRequest(new ApiError
                {
                    Code = ErrorCodes.InvalidInput,
                    Msg = "Sorry, only day candles are available (temporary)."
                });
            }

            var pairs = (await _assetPairDictionary.Values()).Where(x => !x.IsDisabled);

            if (request.AssetPairIds.Any(x => !pairs.Select(y => y.Id).Contains(x)))
            {
                return BadRequest(new ApiError
                {
                    Code = ErrorCodes.InvalidInput,
                    Msg = "Unkown asset pair id present"
                });
            }

            var feeds = new List<IFeedHistory>();
            var result = new List<AssetPairHistoryRateModel>();

            foreach (var pairId in request.AssetPairIds)
            {
                var askFeed = await _feedHistoryRepository.GetСlosestAvailableAsync(pairId, TradePriceType.Ask, request.DateTime);
                var bidFeed = await _feedHistoryRepository.GetСlosestAvailableAsync(pairId, TradePriceType.Bid, request.DateTime);

                if (askFeed != null && bidFeed != null)
                {
                    feeds.Add(askFeed);
                    feeds.Add(bidFeed);
                }
                else
                {
                    //add empty candles
                    result.Add(new AssetPairHistoryRateModel {Id = pairId});
                }
            }

            result.AddRange(feeds.ToApiModel());

            return Ok(result);
        }


        /// <summary>
        /// Get rates for specified period and asset pair
        /// </summary>
        /// <remarks>
        /// Available period values
        ///  
        ///     "Sec",
        ///     "Minute",
        ///     "Hour",
        ///     "Day",
        ///     "Month",
        /// 
        /// </remarks>
        /// <param name="assetPairId">Asset pair Id</param>
        /// <param name="request">Request</param>
        [HttpPost("rate/history/{assetPairId}")]
        public async Task<AssetPairHistoryRateModel> GetHistoryRate([FromRoute]string assetPairId,
            [FromBody] AssetPairRateHistoryRequest request)
        {
            IFeedCandle buyCandle = null;
            IFeedCandle sellCandle = null;

            try
            {
                buyCandle = await _feedCandlesRepository.GetCandleAsync(assetPairId, request.Period.ToDomainModel(),
                    Domain.Prices.PriceType.Bid, request.DateTime);

                sellCandle = await _feedCandlesRepository.GetCandleAsync(assetPairId, request.Period.ToDomainModel(),
                    Domain.Prices.PriceType.Ask, request.DateTime);
            }
            catch (AppSettingException)
            {
                // TODO: Log absent connection string for the specified assetPairId
            }

            return AssetPairHistoryRateModelConvertions.ToApiModel(assetPairId, buyCandle, sellCandle);
        }
    }
}