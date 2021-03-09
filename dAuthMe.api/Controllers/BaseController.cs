using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dAuthMe.api.Tools;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace dAuthMe.api.Controllers
{
    public abstract class BaseController<TEntity> : ControllerBase where TEntity : IEntity
    {
        protected readonly IBaseRepository<TEntity> _repo;

        public BaseController(IBaseRepository<TEntity> repo) => _repo = repo;

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TEntity>> Get([FromRoute] string id) {
            if (!ObjectId.TryParse(id, out var objid)) return BadRequest($"'{id}' is not a valid 24 digit hex string");

            return await _repo.Get(new ObjectId(id));
        }

        [HttpGet]
        public virtual async Task<List<TEntity>> Get() => await _repo.Get();

        [HttpPost]
        public virtual async Task<ActionResult> Create([FromBody] TEntity model) {
            try
            {
                await _repo.Create(model);
            }
            catch (CustomException e)
            {
                return BadRequest(e.Message); 
            }

            return Ok();
        }

        [HttpPut]
        public virtual async Task<ActionResult> Update([FromBody] TEntity model) {
            try
            {
                await _repo.Update(model);
            }
            catch (CustomException e)
            {
                
                return BadRequest(e.Message); 
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public virtual async Task Delete([FromRoute] string id) => await _repo.Delete(new ObjectId(id));
    }
}
