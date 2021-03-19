using System;
using System.Threading.Tasks;
using dAuthMe.api.Models;
using MongoDB.Driver;

namespace dAuthMe.api.Controllers
{
    public interface IUserRepository: IBaseRepository<UserModel>
    {
        Task<UserModel> Get(Guid username);
    }

    public class UserRepository: BaseRepository<UserModel>, IUserRepository
    {
        public UserRepository(IRepoDBContext context): base(context) {}

        public async Task<UserModel> Get(Guid username)
        {
            var cursor = await _collection.FindAsync(Filter.Eq("Username", username));
            return await cursor.FirstOrDefaultAsync();
        }
    }
}
