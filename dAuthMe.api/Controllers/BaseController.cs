using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dAuthMe.api.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace dAuthMe.api.Controllers
{
    public abstract class BaseController<TEntity> : ControllerBase where TEntity : IEntity
    {
        protected readonly IBaseRepository<TEntity> _repo;
        private readonly ILogger _loger;

        private string _modelName => typeof(TEntity).Name;
        
        public BaseController(IBaseRepository<TEntity> repo, ILogger logger) {
            _repo = repo;
            _loger = logger;
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TEntity>> Get([FromRoute] string id) {
            if (!ObjectId.TryParse(id, out var objid)) return BadRequest($"'{id}' is not a valid 24 digit hex string");

            return await _repo.Get(new ObjectId(id));
        }

        [HttpGet]
        public virtual async Task<List<TEntity>> Get() => await _repo.Get();

        [HttpPost]
        public virtual async Task<ActionResult<TEntity>> Create([FromBody] TEntity model) {
            if (model == null)
                return BadRequest($"{_modelName} is null");

            try
            {
                await _repo.Create(model);
                _loger.LogInformation($"{_modelName} with id {model.GetId()} was created");
            }
            catch (CustomException e)
            {
                return BadRequest(e.Message); 
            }

            return model;
        }

        [HttpPut]
        public virtual async Task<ActionResult<TEntity>> Update([FromBody] TEntity model) {
            if (model == null)
                return BadRequest($"{_modelName} is null");

            try
            {
                await _repo.Update(model);
                _loger.LogInformation($"{_modelName} with id {model.GetId()} was updated");
            }
            catch (CustomException e)
            {
                
                return BadRequest(e.Message); 
            }

            return model;
        }

        [HttpDelete("{id}")]
        public virtual async Task Delete([FromRoute] string id) {
            await _repo.Delete(new ObjectId(id));

            _loger.LogInformation($"{_modelName} with id {id} was deleted");
        }
    }
}
