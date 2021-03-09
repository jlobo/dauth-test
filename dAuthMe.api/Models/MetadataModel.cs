using System;
using System.Collections.Generic;
using dAuthMe.api.Controllers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dAuthMe.api.Models
{
    public class MetadataModel : IEntity
    {

        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string Userid { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        //TODO: Manage to get any type in value for mongodb 
        public Dictionary<string, string> Classifications { get; set; }

        public ObjectId GetId() => new ObjectId(Id);
    }
}
