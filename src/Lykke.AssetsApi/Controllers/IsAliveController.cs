using System;
using Lykke.AssetsApi.Models.IsAlive;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.PlatformAbstractions;

namespace Lykke.AssetsApi.Controllers
{
    /// <summary>
    /// Controller to test service is alive
    /// </summary>
    [Route("api/[controller]")]
    public class IsAliveController : Controller
    {
        /// <summary>
        /// Checks service is alive
        /// </summary>
        [HttpGet]
        public IsAliveResponse Get()
        {
            return new IsAliveResponse
            {
                Version = PlatformServices.Default.Application.ApplicationVersion,
                Env = Environment.GetEnvironmentVariable("Env")
            };
        }
    }
}