using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace dAuthMe.api.Controllers
{
    public abstract class BaseController<TEntity> : ControllerBase where TEntity : IEntity
    {
        protected readonly IBaseRepository<TEntity> _repo;

        public BaseController(IBaseRepository<TEntity> repo) => _repo = repo;

        [HttpGet("{id}")]
        public async Task<ActionResult<TEntity>> Get([FromRoute] string id) {
            if (!ObjectId.TryParse(id, out var objid)) return BadRequest($"'{id}' is not a valid 24 digit hex string");

            return await _repo.Get(new ObjectId(id));
        }

        [HttpGet]
        public async Task<List<TEntity>> Get() => await _repo.Get();

        [HttpPost]
        public async Task Create([FromBody] TEntity model) {
            await _repo.Create(model);
        }

        [HttpPut]
        public async Task Update([FromBody] TEntity model) => await _repo.Update(model);

        [HttpDelete("{id}")]
        public async Task Delete([FromRoute] string id) => await _repo.Delete(new ObjectId(id));
    }
}
