using MongoDB.Bson;

namespace dAuthMe.api.Controllers
{
    public interface IEntity {
        ObjectId GetId();
    }
}
