using System;
using System.Collections.Generic;
using tempaastapi.Models;
using tempaastapi.repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using tempaastapi.attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;

namespace tempaastapi.Controllers
{
    [ApiController]
    [Route("api/key")]
    public class ApiKeyController : ControllerBase
    {
        private readonly ILogger<ApiKeyController> _logger;
        private IApiKey _akr;
        IConfiguration _config;
        public ApiKeyController(ILogger<ApiKeyController> logger, IApiKey akr, IConfiguration config)
        {
            _logger = logger;
            _akr = akr;
            _config = config;
        }

        [HttpGet("")]
        [Authorize]
        public List<ApiKeyEntity> Get()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value.Split("|")[1];
            Console.WriteLine(userId);
            return _akr.GetApiKeyByUser(userId);

        }

        [HttpGet("validate")]
        [ApiKey]
        public ApiKeyEntity ValidateApiKey()
        {
            Request.Headers.TryGetValue("Api-Key", out var apiKey);
            var keys = _akr.GetApiKeyByUser(_config["UserId"]);
            return keys.Where(k => k.RowKey == apiKey).FirstOrDefault<ApiKeyEntity>();
        }

        [HttpPost("")]
        [Authorize]
        public ApiKeyEntity GenerateApiKey()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value.Split("|")[1];
            return _akr.GenerateApiKey(userId);
        }

        [HttpDelete]
        [Authorize]
        public IActionResult DeleteApiKey(string apiKey)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value.Split("|")[1];
                _akr.DeleteApiKey(userId, apiKey);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
