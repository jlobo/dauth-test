using System;
using dAuthMe.api.Controllers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dAuthMe.api.Models
{
    public class VendorModel : IEntity
    {

        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public String Id { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public String ParentId { get; set; }
        public string Name { get; set; }
        // TODO: Account must be unique 
        public string Account { get; set; }
        public Uri CallbackUrl { get; set; }
        public byte[] JwtPublicKey { get; set; }
        public byte[] JwtPrivateKey { get; set; }
        public byte[] TidePrivateKey { get; set; }
        public Uri Logo { get; set; }
        public Uri[] Orks { get; set; }
        
        public ObjectId GetId() => new ObjectId(Id);
    }
}
