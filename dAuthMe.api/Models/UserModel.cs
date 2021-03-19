using System;
using System.Linq;
using System.Security.Cryptography;
using dAuthMe.api.Controllers;
using dAuthMe.api.Tools;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dAuthMe.api.Models
{
    public class UserModel : IEntity
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string Vendorid { get; set; }

        public Guid Username { get; set; }
        public string PublicKey { get; set; }
        public DateTimeOffset Joined { get; set; }
        public DateTimeOffset Modified { get; set; }
        public DateTimeOffset Logged { get; set; }
        public string[] Roles { get; set; }

        public ObjectId GetId() => new ObjectId(Id);

        public ECDsa GetPublicKey() => ECDsaSerializer.GetEcdsa(PublicKey, true);
    }
}
