using System;
using dAuthMe.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace dAuthMe.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VendorController : BaseController<VendorModel>
    {
        public VendorController(IBaseRepository<VendorModel> repo) : base(repo) { }
    }
}
