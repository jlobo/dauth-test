using System;
using dAuthMe.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace dAuthMe.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MetadataController : BaseController<MetadataModel>
    {
        public MetadataController(IBaseRepository<MetadataModel> repo) : base(repo) { }
    }
}
