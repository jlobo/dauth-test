using System;
using dAuthMe.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace dAuthMe.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MetadataController : BaseController<MetadataModel>
    {
        public MetadataController(IBaseRepository<MetadataModel> repo, ILogger<MetadataController> logger) : base(repo, logger) { }
    }
}
