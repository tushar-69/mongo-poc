using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace MongoPOC.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [MinLength(3)]
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("price")]
        [Range(0, double.MaxValue)]
        public double Price { get; set; }

        [MinLength(3)]
        [BsonElement("category")]
        public string Category { get; set; }

        [BsonElement("reviews")]
        public List<Review> Reviews { get; set; }
    }

    public class Review
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("rating")]
        public int Rating { get; set; }
    }
}
