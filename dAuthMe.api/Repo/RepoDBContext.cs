using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace dAuthMe.api.Controllers {
    public interface IRepoDBContext
    {
        IMongoCollection<Book> GetCollection<Book>(string name);
    }

    public class RepoDBContext : IRepoDBContext
    {
        protected readonly IMongoDatabase _db;
        protected readonly IMongoClient _client;
        public IClientSessionHandle Session { get; set; }

        public RepoDBContext(IOptions<MongoSettings> config)
        {
            _client = new MongoClient(config.Value.ConnectionString);
            _db = _client.GetDatabase(config.Value.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            return _db.GetCollection<T>(name);
        }
    }
}

