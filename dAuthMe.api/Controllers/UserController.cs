using System;
using System.Threading.Tasks;
using dAuthMe.api.Models;
using dAuthMe.api.Tools;
using DnsClient.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace dAuthMe.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : BaseController<UserModel>
    {
        private new readonly IUserRepository _repo;
        private readonly ApiSettings _config;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserRepository repo, IOptions<ApiSettings> config, ILogger<UserController> logger) : base(repo, logger) {
            _repo = repo;
            _config = config.Value;
            _logger = logger;
        }

        public override Task<ActionResult<UserModel>> Create([FromBody] UserModel model)
        {
            if (model != null) {
                model.Joined = DateTimeOffset.Now;
                model.Modified = DateTimeOffset.Now;
            }

            _logger.LogInformation($"thi sis my joined {model.Joined}");
            return base.Create(model);
        }

        //TODO: Add authorization
        [Authorize]
        public override Task<ActionResult<UserModel>> Update([FromBody] UserModel model) => base.Update(model);

        [AllowAnonymous]
        [HttpGet("token")]
        public async Task<ActionResult> Token([FromHeader] string authorization)
        {
            if (string.IsNullOrWhiteSpace(authorization))
            {
                _logger.LogInformation("Invalid token: Empty token");
                return BadRequest();
            }

            if (!JwtToken.TryParse(authorization, out var jwt))
            {
                _logger.LogInformation("Invalid token: Invalid token format");
                return BadRequest();
            }

            if (string.IsNullOrWhiteSpace(jwt.Payload.Sub))
            {
                _logger.LogInformation("Invalid token: Token without subject");
                return BadRequest();
            }

            if (!Guid.TryParse(jwt.Payload.Sub, out var username))
            {
                _logger.LogInformation("Invalid token: invalid sub");
                return BadRequest("invalid token");
            }

            var user = await _repo.Get(username);
            var delayCheck = Task.Delay(500);
            if (user == null)
            {
                _logger.LogInformation("Invalid authentication: user '{0}' not found", username);
                await delayCheck;
                return Unauthorized("Account not found or invalid token");
            }

            //TODO: window token validation of time
            if (!JwtToken.FromECDsa(user.GetPublicKey()).Verify(jwt.RawData))
            {
                _logger.LogInformation("Invalid authentication: invalid token for vendor '{0}", username);
                await delayCheck;
                return Unauthorized("Account not found or invalid token");
            }

            var newToken = JwtToken.FromHmac(_config.SecretKey).AddAudience(jwt.Payload.Sub).Generate(TimeSpan.FromDays(365));
            _logger.LogInformation("Valid authentication: for vendor '{0}", username);
            await delayCheck;
            return Ok(newToken);
        }
    }
}
