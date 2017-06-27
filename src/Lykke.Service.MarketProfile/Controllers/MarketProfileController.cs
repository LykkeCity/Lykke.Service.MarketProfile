using System.Collections.Generic;
using System.Linq;
using System.Net;
using Lykke.Service.MarketProfile.Core.Services;
using Lykke.Service.MarketProfile.Models;
using Lykke.Service.MarketProfile.Models.MarketProfile;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.MarketProfile.Controllers
{
    [Route("api/[controller]")]
    public class MarketProfileController : Controller
    {
        private readonly IMarketProfileManager _manager;

        public MarketProfileController(IMarketProfileManager manager)
        {
            _manager = manager;
        }

        [HttpGet("")]
        public IEnumerable<AssetPairModel> GetAll()
        {
            var pairs = _manager.GetAllPairs();

            return pairs.Select(p => p.ToApiModel());
        }

        [HttpGet("{pairCode}")]
        [ProducesResponseType(typeof(AssetPairModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorModel), (int)HttpStatusCode.BadRequest)]
        public IActionResult Get(string pairCode)
        {
            if (string.IsNullOrWhiteSpace(pairCode))
            {
                return BadRequest(new ErrorModel
                {
                    Code = ErrorCode.InvalidInput,
                    Message = "Pair code is required"
                });
            }

            var pair = _manager.TryGetPair(pairCode);

            if (pair == null)
            {
                return NotFound(new ErrorModel
                {
                    Code = ErrorCode.PairNotFound,
                    Message  = "Pair not found"
                });
            }

            return Ok(pair.ToApiModel());
        }
    }
}