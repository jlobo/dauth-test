using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dAuthMe.api.Tools;
using MongoDB.Bson;
using MongoDB.Driver;

namespace dAuthMe.api.Controllers
{
    public interface IBaseRepository<TEntity> where TEntity : IEntity
    {
        Task Create(TEntity model);
        Task Update(TEntity model);
        Task Delete(ObjectId id);
        Task<TEntity> Get(ObjectId id);
        Task<List<TEntity>> Get();
    }

    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : IEntity
    {
        protected readonly IMongoCollection<TEntity> _collection;

        public static FilterDefinitionBuilder<TEntity> Filter => Builders<TEntity>.Filter;

        public BaseRepository(IRepoDBContext context)
        {
            _collection = context.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public async Task<TEntity> Get(ObjectId id)
        {
            var cursor = await _collection.FindAsync(Filter.Eq("_id", id));
            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<List<TEntity>> Get()
        {
            var cursor = await _collection.FindAsync(Filter.Empty);
            return await cursor.ToListAsync();
        }

        public async Task Create(TEntity model)
        {
            if (model == null) throw new CustomException(typeof(TEntity).Name + " object is null");

            try
            {
                await _collection.InsertOneAsync(model);
            }
            catch (MongoException e)
            {
                if (e.Message.Contains("E11000")) {
                    throw new CustomException("Id already exists", e);
                }
                
                throw new Exception("Unknown exception", e);
            }
        }

        public async Task Update(TEntity model)
        {
            await _collection.ReplaceOneAsync(Filter.Eq("_id", model.GetId()), model);
        }

        public async Task Delete(ObjectId id)
        {
            await _collection.DeleteOneAsync(Filter.Eq("_id", id));
        }
    }
}
