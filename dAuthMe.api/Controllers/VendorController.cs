using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using dAuthMe.api.Models;
using dAuthMe.api.Tools;
using DnsClient.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
namespace dAuthMe.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VendorController : BaseController<VendorModel>
    {
        const string rex = "[Bb]earer\\s+([a-zA-Z0-9\\-_]+\\.[a-zA-Z0-9\\-_]+(?:\\.[a-zA-Z0-9\\-_]+)?)";

        private readonly ILogger<VendorController> _logger;
        private readonly ApiSettings _config;

        public VendorController(IBaseRepository<VendorModel> repo, IOptions<ApiSettings> config, ILogger<VendorController> logger) : base(repo) {
            _logger = logger;
            _config = config.Value;
        }

        [AllowAnonymous]
        [HttpGet("token")]
        public async Task<ActionResult> Token([FromHeader] string authorization)
        {
            if (string.IsNullOrWhiteSpace(authorization))
            {
                _logger.LogInformation("Invalid token: Empty token");
                return BadRequest();
            }

            var match = Regex.Match(authorization, rex);
            if (match.Groups.Count < 2)
            {
                _logger.LogInformation("Invalid token: Invalid token format");
                return BadRequest();
            }

            var jwt = new JwtSecurityToken(match.Groups[1].Value);
            if (string.IsNullOrWhiteSpace(jwt.Payload.Sub))
            {
                _logger.LogInformation("Invalid token: Token without subject");
                return BadRequest();
            }

            if (!ObjectId.TryParse(jwt.Payload.Sub, out var id)) {
                _logger.LogInformation("Invalid token: invalid sub");
                return BadRequest("invalid token");
            }

            var vendor = await this._repo.Get(id);
            var delayCheck = Task.Delay(500);
            if (vendor == null) {
                _logger.LogInformation("Invalid authentication: vendor '{0}' not found", id);
                await delayCheck;
                return Unauthorized("Account not found or invalid token");
            }

            //TODO: window token validation of time
            if (!JwtToken.FromECDsa(vendor.JwtPublicKey, false).Verify(match.Groups[1].Value)) {
                _logger.LogInformation("Invalid authentication: invalid token for vendor '{0}", id);
                await delayCheck;
                return Unauthorized("Account not found or invalid token");
            }
            
            var newToken = JwtToken.FromHmac(_config.SecretKey).AddAudience(jwt.Payload.Sub).Generate(TimeSpan.FromDays(365));
            _logger.LogInformation("Valid authentication: for vendor '{0}", id);
            await delayCheck;
            return Ok(newToken);
        }
    }
}
